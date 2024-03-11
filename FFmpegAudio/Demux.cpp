#include "pch.h"
#include "Demux.h"
//#include "Log.h"
#include "common.h"
#include <map>

using std::format;
namespace media
{
	std::atomic_bool demux_abort = false;

}
media::Demuxer::Demuxer(std::shared_ptr<AVPacketQueue> audio_pkt_queue,std::string url)
	:_audio_queue(audio_pkt_queue), _url(url)
{
	//Log::debug(format("解复用构造：{}",_url));
}

media::Demuxer::~Demuxer()
{
	//Log::debug("解复用销毁");
	avformat_close_input(&ifmt_ctx);
	delete work_thread;
}

int media::Demuxer::init()
{
	clean_queue();

	int ret = 0;
	ifmt_ctx = avformat_alloc_context();
	if (!ifmt_ctx)
	{
		//Log::error("avformat_alloc_context failed!");
		return -1;
	}
	ret = avformat_open_input(&ifmt_ctx, _url.c_str(), nullptr, nullptr);
	if (ret != 0)
	{
		av_strerror(ret, _error_str, sizeof(_error_str));
		//Log::error(format("打开文件失败：{}",_error_str));
		return -1;
	}
	ret = avformat_find_stream_info(ifmt_ctx, nullptr);
	if (ret != 0)
	{
		av_strerror(ret, _error_str, sizeof(_error_str));
		//Log::error(format("流信息获取失败：{}", _error_str));
		return -1;
	}
	//av_dump_format(ifmt_ctx,0,_url.c_str(),0);
	_audio_index = av_find_best_stream(ifmt_ctx, AVMEDIA_TYPE_AUDIO, -1, -1, nullptr, 0);

	if (_audio_index == -1)
	{
		//Log::error("没有找到音频流！");
		return -1;
	}
	//Log::debug("解复用初始化完成");
	demux_abort = false;
	return ret;
}

void media::Demuxer::start()
{
	work_thread = new std::jthread(&Demuxer::run, this);

	if (!work_thread)
	{
		//Log::error("new std::thread failed");
		return;
	}
}

int media::Demuxer::run()
{
	//Log::debug("解复用线程运行中");
	int ret = 0;
	while (!demux_abort.load())
	{
		AVPacket pkt;
		if (_audio_queue->size() > 100 && !demux_abort.load())
		{
			//Log::debug("包队列已满，请解包播放");
			std::this_thread::sleep_for(std::chrono::milliseconds(10));
			continue;
		}
		ret = av_read_frame(ifmt_ctx, &pkt);
		if (ret == AVERROR_EOF)
		{
			//解复用结束了
			//Log::debug("解复用线程 av_read_frame 文件尾");
			demux_abort = true;
	
		}
		else if (ret < 0)
		{
			av_strerror(ret, _error_str, sizeof(_error_str));
			//Log::error(format("av_read_frame：{}", _error_str));
			demux_abort = true;
		}
		if (pkt.stream_index == _audio_index)
		{
			//Log::debug(format("DECODE thread __audio_queue is: {}", (int)&_audio_queue));
			_audio_queue->push(&pkt);
			
			
			//Log::debug(format("audio pkt queue size: {}",(int)(this->_audio_queue->size())));
		}
		else
		{
			av_packet_unref(&pkt);
		}
	}
	//Log::debug("解复用线程退出");
	return 0;
}

int media::Demuxer::seek(double target_time)
{

	double _target_time = target_time * AV_TIME_BASE;
	auto target_timestamp = av_rescale_q(_target_time, AV_TIME_BASE_Q, audio_stream_timebase());
	av_seek_frame(ifmt_ctx, _audio_index, target_timestamp, AVSEEK_FLAG_BACKWARD);

	return 0;
}

unsigned int media::Demuxer::get_audio_seconds()
{
	if (_audio_index >= 0)
	{
		int64_t duration = ifmt_ctx->streams[_audio_index]->duration;
		auto time_base = ifmt_ctx->streams[_audio_index]->time_base;
		return duration * time_base.num / time_base.den;
	}
	return 0;
}

AVCodecParameters* media::Demuxer::audio_codec_parameters()
{
	if (_audio_index >= 0)
	{
		return ifmt_ctx->streams[_audio_index]->codecpar;
	}
	return nullptr;
}

std::map<std::string, std::string> media::Demuxer::get_metadata() const
{
	std::map<std::string, std::string> metadata;
	AVDictionaryEntry* tag = nullptr;
	if (_audio_index >= 0)
	{
		auto _metadata = ifmt_ctx->metadata;
		if (!_metadata)
			_metadata = ifmt_ctx->streams[_audio_index]->metadata;
		while (tag = av_dict_get(_metadata, "", tag, AV_DICT_IGNORE_SUFFIX))
		{
			std::string key(tag->key), value(tag->value);
			std::transform(key.begin(), key.end(), key.begin(), [](unsigned char c) {return std::tolower(c); });
			std::transform(value.begin(), value.end(), value.begin(), [](unsigned char c) {return std::tolower(c); });
			metadata.insert({ key,value });
		}
	}
	return metadata;
}
AVRational media::Demuxer::audio_stream_timebase()
{
	if (_audio_index >= 0)
	{
		return ifmt_ctx->streams[_audio_index]->time_base;
	}
	return AVRational{0,0};
}

void media::Demuxer::clean_queue()
{
	clean_audio_queue();
}

void media::Demuxer::clean_audio_queue()
{
	while (!_audio_queue->empty()) _audio_queue->pop();
}

