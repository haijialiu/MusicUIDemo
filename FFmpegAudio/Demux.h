#pragma once
#include "MediaQueue.h"
#include "common.h"
#include <map>
#ifdef _WIN32
#include <windows.h>
#include <processthreadsapi.h>
#endif

namespace media {


	class Demuxer
	{
	public:
		Demuxer(std::shared_ptr<AVPacketQueue> audio_pkt_queue, std::string url);
		~Demuxer();
		int init();
		void start();
		int run();
		void set_name()
		{

#ifdef _WIN32
			HRESULT r;
			r = SetThreadDescription(GetCurrentThread(), L"解复用线程");
#endif
		}
		int seek(double target_time);
		unsigned int get_audio_seconds();

		std::map<std::string,std::string> get_metadata() const;
		AVCodecParameters* audio_codec_parameters();
		AVRational audio_stream_timebase();
	private:
		void clean_queue();
		void clean_audio_queue();
		std::string _url;
		char _error_str[256]{ 0 };
		std::shared_ptr<AVPacketQueue> _audio_queue = nullptr;

		AVFormatContext* ifmt_ctx = nullptr;
		int _audio_index = -1;
		std::jthread* work_thread;
	};
}

