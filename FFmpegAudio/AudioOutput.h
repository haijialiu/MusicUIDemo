#pragma once
#include "MediaQueue.h"
#include <SDL2/SDL.h>
extern "C"
{
#include <libavcodec/avcodec.h>
#include <libswscale/swscale.h>
#include <libswresample/swresample.h>
}
constexpr auto AVCODEC_MAX_AUDIO_FRAME_SIZE = 192000;

namespace media {
    extern std::atomic_bool audio_play_over;

    class AudioParams
    {
    public:
        int freq;
        AVChannelLayout ch_layout;
        enum AVSampleFormat fmt;
        int frame_size;
    };
    class AudioOutput
    {
        friend void fill_audio_pcm(void* userdata, Uint8* streak, int len);
    public:
        AudioOutput(AVRational time_base, const AudioParams& audio_params, std::shared_ptr<AVFrameQueue> frame_queue);
        ~AudioOutput();
        int init();
        int destory();
        int pause();
        int play();
        int set_volume(int vol);
        int get_volume();
        double* get_pts();
    private:

        double* play_seconds = nullptr;
        int64_t _pts = AV_NOPTS_VALUE;
        AudioParams _src_tgt; //解码后的参数
        AudioParams _dst_tgt; //实际输出的格式

        std::shared_ptr<AVFrameQueue> _frame_queue = nullptr;
        struct SwrContext* _swr_ctx = nullptr;
        AVRational _time_base;
    };
}
