#include "pch.h"
#include "AudioOutput.h"
#ifdef _DEBUG
#include "Log.h"
#endif
#include <fstream>
#include <format>
namespace media
{
    std::atomic_bool audio_play_over =  false;
    std::atomic_bool audio_play_abort =  false;
    std::atomic_bool audio_play_abort_confirm = false;
    std::atomic_bool audio_shange_volume_event = false;

}
media::AudioOutput::AudioOutput(AVRational time_base,const AudioParams& audio_params, std::shared_ptr<AVFrameQueue> frame_queue)
    : _time_base(time_base), _src_tgt(audio_params), _frame_queue(frame_queue)
{
    play_seconds = new double(0);
    audio_play_over = false;
    audio_play_abort = false;
    audio_play_abort_confirm = false;
}

media::AudioOutput::~AudioOutput()
{
    delete play_seconds;
    play_seconds = nullptr;
    SDL_PauseAudio(1);

    SDL_CloseAudio();
#ifdef _DEBUG
    Log::debug("AudioOutput::~AudioOutput");
#endif
}
std::ofstream dump_pcm_cpp("dump_cpp.pcm",std::ios::out|std::ios::binary);
void media::fill_audio_pcm(void* userdata,Uint8* stream,int len)
{
    AudioOutput* data = (AudioOutput*)userdata;
    unsigned int len1, audio_size = 0, audio_size1 = 0;
    static uint8_t audio_buf[(AVCODEC_MAX_AUDIO_FRAME_SIZE * 3) / 2]; //存放PCM数据
    static uint8_t audio_buf1[(AVCODEC_MAX_AUDIO_FRAME_SIZE * 3) / 2]; //存放PCM重采样数据
    static unsigned int audio_buf_size = 0;
    static unsigned int audio_buf_index = 0;
    while (len > 0 && !audio_play_abort)
    {
        bool no_data = false;
        bool re_sample = false;
        //这个PCM缓冲区已经读完了
        if (audio_buf_index == audio_buf_size)
        {
            audio_buf_index = 0;

            AVFrame* frame = data->_frame_queue->pop(10).value_or(nullptr);

            if (frame)
            {
                data->_pts = frame->pts;
                //重采样
                if (((frame->format != data->_dst_tgt.fmt)
                    || (frame->sample_rate != data->_dst_tgt.freq)
                    || av_channel_layout_compare(&frame->ch_layout, &data->_dst_tgt.ch_layout))
                    && (!data->_swr_ctx))
                {
                    swr_alloc_set_opts2(&data->_swr_ctx, &data->_dst_tgt.ch_layout,
                        (enum AVSampleFormat)data->_dst_tgt.fmt,
                        data->_dst_tgt.freq, &frame->ch_layout,
                        (enum AVSampleFormat)frame->format, frame->sample_rate, 0, nullptr);
                    if (!data->_swr_ctx || swr_init(data->_swr_ctx) < 0)
                    {
#ifdef _DEBUG
                        Log::error("重采样器构建失败");
#endif
                        swr_free((SwrContext**)(&data->_swr_ctx));
                        return;
                    }
                }
                if (data->_swr_ctx)//准备重采样
                {
                    const uint8_t** in = (const uint8_t**)frame->extended_data;
                    uint8_t* out = audio_buf;
                    int out_samples = frame->nb_samples * data->_dst_tgt.freq / frame->sample_rate + 256;
                    int out_bytes = av_samples_get_buffer_size(nullptr, data->_dst_tgt.ch_layout.nb_channels, out_samples, data->_dst_tgt.fmt, 0);
                    if (out_bytes < 0) {
#ifdef _DEBUG
                        Log::error("av_samples_get_buffer_size failed");
#endif
                        return;
                    }
                    int len2 = swr_convert(data->_swr_ctx, &out, out_samples, in, frame->nb_samples); // 返回样本数量
                    if (len2 < 0) {
#ifdef _DEBUG
                        Log::error("swr_convert failed");
#endif
                        return;
                    }
                    audio_buf_size = av_samples_get_buffer_size(NULL, data->_dst_tgt.ch_layout.nb_channels, len2, data->_dst_tgt.fmt, 1);
                    memcpy(audio_buf, frame->data[0], audio_size);

                } 
                else
                {
                    //没有重采样
                    audio_size = av_samples_get_buffer_size(nullptr, frame->ch_layout.nb_channels, frame->nb_samples, (enum AVSampleFormat)frame->format, 1);
                    memcpy(audio_buf, frame->data[0], audio_size);
                    audio_buf_size = audio_size;
                }
                av_frame_free(&frame);
            }
            else
            {
                no_data = true;
                audio_buf_size = 512;
            }
        }

        len1 = audio_buf_size - audio_buf_index;
        if (len1 > len)
            len1 = len;
        if (no_data)
        {
            memset(stream, 0, len1);
        }
        else
        {
            //assert(audio_buf_index + len1 <= (AVCODEC_MAX_AUDIO_FRAME_SIZE * 3) / 2);
            memcpy(stream, audio_buf + audio_buf_index, len1);
            //输出pcm
            dump_pcm_cpp.write((char*)audio_buf + audio_buf_index, len1);
            dump_pcm_cpp.flush();
        }
        len -= len1;
        stream += len1;
        audio_buf_index += len1;
        auto pts = data->_pts * av_q2d(data->_time_base);
        if(data->play_seconds)
            *data->play_seconds = pts;
#ifdef _DEBUG
        Log::error(std::format("audio pts:{:02}:{:.3f}",(int)pts/60,fmod(pts,60)));
#endif
        //std::cout << std::format("audio pts:{:02}:{:.3f}", (int)pts / 60, fmod(pts, 60)) << "\r";
        
    }
}

int media::AudioOutput::init()
{
    audio_play_abort = false;
    if (SDL_Init(SDL_INIT_AUDIO) != 0)
    {
#ifdef _DEBUG
        Log::error("SDL init failed");
#endif
        return -1;
    }
    SDL_AudioSpec wanted_spec{ 0 }, spec{0};
    wanted_spec.channels = _src_tgt.ch_layout.nb_channels;
    wanted_spec.freq = _src_tgt.freq;
    wanted_spec.format = AUDIO_S16SYS;
    wanted_spec.silence = 0;
    wanted_spec.callback = fill_audio_pcm;
    wanted_spec.userdata = this;
    wanted_spec.samples = 1024; // 采样数量

    int ret = SDL_OpenAudio(&wanted_spec,nullptr);
    if (ret < 0)
    {
#ifdef _DEBUG
        Log::error("SDL_OpenAudio error");
#endif
        return -1;
    }
    if (wanted_spec.format != AUDIO_S16SYS) {
#ifdef _DEBUG
        Log::error("SDL format failed");
#endif
        return -1;
    }
    
    av_channel_layout_default(&_dst_tgt.ch_layout, wanted_spec.channels);
    _dst_tgt.fmt = AV_SAMPLE_FMT_S16;
    _dst_tgt.freq = wanted_spec.freq;
    _dst_tgt.frame_size = wanted_spec.samples;
    SDL_PauseAudio(0);
    Log::debug("AudioOutput::init over");
    return 0;
}

int media::AudioOutput::destory()
{
    SDL_PauseAudio(1);
    SDL_CloseAudio();
    Log::debug("AudioOutput::destory");
    return 0;
}

int media::AudioOutput::pause()
{
    SDL_PauseAudio(1);
    return 0;
}

int media::AudioOutput::play()
{
    SDL_PauseAudio(0);
    return 0;
}

int media::AudioOutput::set_volume(int vol)
{
    
    return 0;
}

int media::AudioOutput::get_volume()
{
    return 0;
}

double* media::AudioOutput::get_pts()
{
    return play_seconds;
}
