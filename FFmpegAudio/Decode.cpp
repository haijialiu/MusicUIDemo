#include "pch.h"
#include "Decode.h"
#ifdef _DEBUG
#include "Log.h"
#endif
#include <format>

using std::format;
namespace media
{
    //这个标志表示解码线程终止了，设为true之后当解码循环跑完当次之后会结束run
    std::atomic_bool decode_abort = false;
    //这个标志表示解码完成了，但播放是否完成就与它无关了
    std::atomic_bool decode_over = false;
    //播放设备关闭状态
    extern std::atomic_bool audio_play_abort;
}
media::Decoder::Decoder(std::shared_ptr<AVPacketQueue> packet_queue, std::shared_ptr<AVFrameQueue> frame_queue)
    :_packet_queue(packet_queue),_frame_queue(frame_queue)
{
    
}

media::Decoder::~Decoder()
{

    avcodec_close(_codec_ctx);
    av_frame_free(&_frame);
    delete work_thread;
#ifdef _DEBUG
    Log::info("Decode::~Decode");
#endif
}

int media::Decoder::init(AVCodecParameters* params)
{ 


    //while (!(*_frame_queue)->empty())
    //{
    //    (*_frame_queue)->pop();
    //}
    _codec_ctx = avcodec_alloc_context3(nullptr);
    _frame = av_frame_alloc();


    if (params == nullptr)
    {
#ifdef _DEBUG
        Log::error("参数为空");
#endif
        return -1;
    }

    int ret = avcodec_parameters_to_context(_codec_ctx, params);
    if (ret < 0)
    {
        av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
        Log::error(format("avcodec_parameters_to_context 失败：{}", _error_str));
#endif
        return -1;
    }
    const AVCodec* codec = avcodec_find_decoder(_codec_ctx->codec_id);
    if (!codec)
    {
        av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
        Log::error(format("avcodec_find_decoder 失败：{}", _error_str));
#endif
        return -1;
    }
    ret = avcodec_open2(_codec_ctx, codec, nullptr);
    if (ret < 0)
    {
        av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
        Log::error(format("avcodec_open2 失败：{}", _error_str));
#endif
        return -1;
    }
    decode_abort = false;
    std::atomic_bool decode_over = false;
#ifdef _DEBUG
    Log::debug("解码初始化完成");
#endif

    return 0;
}


int media::Decoder::start()
{
    work_thread = new std::jthread(&Decoder::run, this);

    if (!work_thread)
    {
#ifdef _DEBUG
        Log::error("new std::thread failed");
#endif
        return -1;
    }
    return 0;
}


int media::Decoder::run()
{
#ifdef _DEBUG
    Log::debug(format("[{}] 解码运行中",debug_name));
#endif
    AVFrame* frame = _frame;
    while (!decode_abort.load()) {

        if (_frame_queue->size() > 10 && !decode_abort.load())
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(10));
            continue;
        }
        auto ret = _packet_queue->pop(10);
        Log::debug(format("DECODE thread _packet_queue is: {}",(int) & _packet_queue));
        AVPacket* pkt = ret.value_or(nullptr);
        
        if (pkt) //获取一个包准备解码
        {
            //将包发给解码器
            int ret = avcodec_send_packet(_codec_ctx, pkt);
            av_packet_free(&pkt);
            if (ret < 0)
            {
                av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
                Log::error(format("{} avcodec_open2 failed：{}",debug_name, _error_str));
#endif
                //发现了一个bug，ogg中断
                //break;
                decode_over = true;
            }
            //从解码器得到解包数据
            while (!decode_over.load())
            {
                ret = avcodec_receive_frame(_codec_ctx, frame);
                if (ret == 0)
                {
                    _frame_queue->push(frame);
                    continue;
                }
                else if (ret == AVERROR(EAGAIN))
                {
                    break;
                }                 
                else if (ret == AVERROR_EOF) //帧读取完了
                {
#ifdef _DEBUG
                    Log::info(format("receive frame：end of file"));
#endif
                    //avcodec_flush_buffers(_codec_ctx);
                    decode_abort = true;
                    break;
                }
                else                //在flac文件中遇到文件尾部会异常
                {
                    av_strerror(ret, _error_str, sizeof(_error_str));
#ifdef _DEBUG
                    Log::error(format("receive frame：{}", _error_str));
#endif
                    decode_abort = true;
                    break;
                }
            }
        }
    }
#ifdef _DEBUG
    Log::debug(format("[{}] 解码结束",debug_name));
#endif
    return 0;
}
void media::Decoder::clean_queue()
{
}
void media::Decoder::clean_frame_queue()
{

}
void media::Decoder::clean_pkt_queue()
{
}