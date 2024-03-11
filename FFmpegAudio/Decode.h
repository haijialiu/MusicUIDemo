#pragma once
#include "MediaQueue.h"

#ifdef _WIN32
#include <windows.h>
#include <processthreadsapi.h>
#endif

namespace media 
{
	extern std::atomic_bool decode_abort;
	
	class Decoder
	{
	public:
		Decoder(std::shared_ptr<AVPacketQueue> packet_queue, std::shared_ptr<AVFrameQueue> frame_queue);
		~Decoder();
		int init(AVCodecParameters* params);
		int start();
		int run();
		void join() { return work_thread->join(); }
		void set_name(std::string name)
		{ 
			debug_name = name;

#ifdef _WIN32
			HRESULT r;
			r = SetThreadDescription(GetCurrentThread(), L"解码线程");
#endif
		}
		std::string debug_name;
	private:
		void clean_queue();
		void clean_frame_queue();
		void clean_pkt_queue();

		char _error_str[256]{ 0 };
		AVCodecContext* _codec_ctx = nullptr;
		std::shared_ptr<AVPacketQueue> _packet_queue = nullptr;
		std::shared_ptr<AVFrameQueue> _frame_queue = nullptr;
		AVFrame* _frame = nullptr;

		std::jthread* work_thread;
	};
}

