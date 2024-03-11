#pragma once
#include "MediaQueue.h"
#include <memory>
#include <any>
#include "Demux.h"
#include "Decode.h"
#include "AudioOutput.h"

namespace media {
	class Player
	{
	public:
		Player(const std::vector<std::string>& music_urls);
		Player() { }
		~Player();
		void set_play_list(const std::vector<std::string>& music_urls);
		int operate(std::string action, std::string value);
		int main_loop();
		int start();
		int play();
		void switch_next(int index);
		void pause();
		void resume();
		void quit();

		void set_play_mode(int mode) { play_mode = mode; }

		//共享当前播放的下标（注意，只允许读不允许写）
		int* share_current_index() { return &_cur_play_index; }		
		bool share_current_status() { return play_status.load(); }
		//共享播放的时间
		double* share_current_play_time()
		{ 
			if(_audio_output)
				return _audio_output->get_pts(); 
			return nullptr;
		}
	private:
		//0：单曲循环，1：顺序播放，2：循环播放 随机播放之后再说
		int play_mode = 1;


		void prepare_audio_player(AVCodecParameters* params);
		void destroy_audio_player();
		void wait_to_play_over();
		void interrupt_current_play();

		bool _abort = false;
		std::atomic_bool play_status = false;
		std::shared_ptr <std::jthread> work_thread = nullptr;
		std::shared_ptr <Demuxer> _demuxer = nullptr;
		std::shared_ptr <Decoder> _audio_decoder = nullptr;
		std::shared_ptr<AudioParams> _audio_params = nullptr;
		std::shared_ptr<AudioOutput> _audio_output = nullptr;
		std::vector<std::string> _music_urls;
		int _cur_play_index = 0;
		static std::shared_ptr<AVPacketQueue> _audio_pkt_queue;	
		static std::shared_ptr<AVFrameQueue> _audio_frame_queue;
	};


}

