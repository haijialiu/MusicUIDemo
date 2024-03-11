#include "pch.h"
#include "Player.h"
#include <thread>
#include <format>
#include <algorithm>
#include <future>

#ifdef _WIN32
#include <windows.h>
#include <processthreadsapi.h>
#endif
namespace media
{
	std::atomic_bool can_display = false;
	std::atomic_bool player_over = false;
	std::atomic_bool no_music = true;
	std::atomic_bool music_pause = true;
	std::atomic_bool first = true;
	std::atomic_bool switch_flag = false;

	extern std::atomic_bool demux_abort;
	extern std::atomic_bool decode_abort;
	extern std::atomic_bool audio_play_abort;

	std::shared_ptr<AVPacketQueue> Player::_audio_pkt_queue = std::make_shared<AVPacketQueue>();
	std::shared_ptr<AVFrameQueue> Player::_audio_frame_queue = std::make_shared<AVFrameQueue>();
	std::future<int> audio_future;
	std::future<int> player_future;

}

media::Player::Player(const std::vector<std::string>& music_urls)
	:_music_urls(music_urls)
{
	//Log::init();
	//Log::debug("======PlayThread contructor=======");

}

media::Player::~Player()
{
	interrupt_current_play();
	player_over.wait(false);
}
void media::Player::set_play_list(const std::vector<std::string>& music_urls)
{

	if (!music_urls.empty())
	{
		_music_urls = music_urls;
		no_music = false;
		no_music.notify_all();
	}
}
//控制器

int media::Player::operate(std::string action, std::string value)
{

	if (action == "switch")
	{

		play_status = true;
		int index = std::stoi(value);
		switch_next(index);
	}
	else if (action == "pause")
	{
		pause();

	}
	else if (action == "resume")
	{

		resume();
	}
	else if (action == "play_mode")
	{

		int mode = std::stoi(value);
		if (mode >= 0 && mode <= 2)
			play_mode = mode;
		else
		{
			//Log::debug("模式选择有误");
			return -1;
		}
	}
	else if (action=="remove")
	{
		int index = std::stoi(value);
		if (index >= 0 && index < _music_urls.size()) 
		{
			_music_urls.erase(_music_urls.begin() + index);
			if (index == _cur_play_index)
			{
				switch_next(index);
			}
		}
	}
	else if (action == "volume")
	{
		//TODO: 音量调节还未实现
	}
	else if (action == "seek")
	{
		//TODO: 拖动进度条（根据难度等各种因素选做）
	}
	else if (action == "system")
	{
		if (value == "quit")
		{
			_abort = true;
		}

	}
	//TODO: 更换歌单还未规划

	if (first.load())
	{
		first = false;
		first.notify_all();
	}
	return 0;
}

int media::Player::main_loop()
{
	//work_thread = new std::jthread(&Player::start, this);
	//Log::init();
	work_thread = std::make_shared<std::jthread>(&Player::start, this);
	if (!work_thread)
	{
		//Log::error("new std::thread failed");
		return -1;
	}
	//while (!_abort)
	//{
	//	//TODO: 打包前记得注释掉
	//	/*std::string action;
	//	std::string value;
	//	std::cin >> action >> value;
	//	operate(action, value);*/
	//}


	return 0;
}

int media::Player::start()
{


#ifdef _WIN32
	HRESULT r;
	r = SetThreadDescription(GetCurrentThread(), L"播放线程");
#endif
	// 播放模式说明： 0 单曲循环 1 顺序播放 2 列表循环
	
	while (!_abort)
	{


		if (_music_urls.empty())
		{
			no_music = true;
			no_music.wait(true);
		}
		first.wait(true);
		play();
		if (switch_flag.load())
		{
			switch_flag = false;
		}
		else if (play_mode == 0)
		{
			//单曲循环
		}
		else if (play_mode == 1)
		{
			_cur_play_index++;
			if (_cur_play_index >= _music_urls.size())
			{
				_cur_play_index = 0;
				pause();

			}
		}
		else if (play_mode == 2)
		{
			_cur_play_index++;
			_cur_play_index = _cur_play_index % _music_urls.size();
		}
	}
	return 0;
}

int media::Player::play()
{

#ifdef _WIN32
	HRESULT r;
	r = SetThreadDescription(GetCurrentThread(), L"播放线程");
#endif
	if (_music_urls.empty())
		return 0;
	player_over = false;
	int ret = -1;
	_demuxer = std::make_shared<Demuxer>(_audio_pkt_queue, _music_urls[_cur_play_index]);
	_demuxer->set_name();
	_audio_decoder = std::make_shared<Decoder>(_audio_pkt_queue, _audio_frame_queue);
	_audio_decoder->set_name("音频");
	ret = _demuxer->init();
	if (ret < 0)
	{
		return ret;
	}
	_demuxer->start();
	//打印信息
	//std::map <std::string, std::string> metadata = _demuxer->get_metadata();
	//auto _titile = metadata.find("title");
	//std::string title = _titile == metadata.end() ? "无标题" : _titile->second;
	//auto _artist = metadata.find("artist");
	//std::string artist = _artist == metadata.end() ? "无歌手" : _artist->second;
	//auto seconds = _demuxer->get_audio_seconds();
	//Log::info(std::format("正在播放: {} - {} {}:{:02}:{:02}", title, artist, seconds / 3600, (seconds % 3600) / 60, seconds % 3600 % 60));


	//2，获取音频解码参数用于解码线程
	//Log::debug("获取音频解码参数");
	auto params = _demuxer->audio_codec_parameters();

	//只放音频的
	prepare_audio_player(params);
	if (first.load())
	{
		_audio_output->pause();
		first = false;
	}
	if (!play_status.load())
	{
		_audio_output->pause();
	}
	//4，加载解码线程
	//Log::info("初始化音频解码线程...");
	_audio_decoder->init(params);
	_audio_decoder->start();

	
	//5，唤醒解复用和解码线程
	//Log::debug("等待音频解码线程...");

	wait_to_play_over();
	//6，销毁播放器（播放器参数尽量贴近音频文件）
	destroy_audio_player();
	//destroy_video_player();
	can_display = false;
	//audio_future.get();
	//Log::debug("==============================一个播放周期结束==============================");

	player_over = true;
	player_over.notify_all();

	return 0;
}

void media::Player::switch_next(int index)
{
	_cur_play_index = index;
	interrupt_current_play();
	if (!music_pause.load())
	{
		player_over.wait(false);
	}



}

void media::Player::pause()
{
	play_status = false;
	if (_audio_output)
	{
		_audio_output->pause();
	}

}

void media::Player::resume()
{
	play_status = true;
	if (_audio_output)
	{
		_audio_output->play();
	}

}

void media::Player::quit()
{
}
void media::Player::prepare_audio_player(AVCodecParameters* params)
{
	//Log::info("初始化播放器");
	_audio_params = std::make_shared<AudioParams>();
	_audio_params->ch_layout = params->ch_layout;
	_audio_params->fmt = (enum AVSampleFormat)params->format;
	_audio_params->freq = params->sample_rate;
	_audio_params->frame_size = params->frame_size;
	_audio_output = std::make_shared<AudioOutput>(_demuxer->audio_stream_timebase(), *_audio_params, _audio_frame_queue);
	//播放器初始化并等待pcm包
	_audio_output->init();
	//Log::info("播放设备已打开");
}

void media::Player::destroy_audio_player()
{
	audio_play_abort = true;
	if (_audio_output)
	{

		_audio_output = nullptr;
		//Log::debug("销毁了音频播放器");
	}
}



void media::Player::wait_to_play_over()
{
	//等待解码器把音频帧全部解完
	//Log::debug("等待音频播放完成...");
	_audio_decoder->join();
	//等待播放器把剩余帧耗尽

	while (!_audio_frame_queue->empty())
	{
		//如果此时播放器都被结束掉了则立即清空
		if (audio_play_abort)
		{
			_audio_frame_queue->pop();
		}
	}
	audio_play_abort = true;
}

void media::Player::interrupt_current_play()
{
	switch_flag = true;
	demux_abort = true;
	decode_abort = true;
	audio_play_abort = true;
	//_audio_output = nullptr;
	//audio_future.get();
	
}


