#pragma once
#ifdef FFMPEGAUDIO_EXPORTS
#define FFMPEGAUDIO_API __declspec(dllexport)
#else
#define FFMPEGAUDIO_API __declspec(dllimport)
#endif // FFMPEGAUDIO_EXPORTS

//初始化播放器
extern "C" FFMPEGAUDIO_API int create_player();
//操作播放器
extern "C" FFMPEGAUDIO_API int operate(const char* action, const char* value);

//当前播放状态
extern "C" FFMPEGAUDIO_API bool share_play_status();
//当前播放时间
extern "C" FFMPEGAUDIO_API double share_play_time();

//当前播放歌曲的下标
extern "C" FFMPEGAUDIO_API int share_play_index();

//注入播放列表
extern "C" FFMPEGAUDIO_API int input_music_urls(const char** music_urls, int num);

//从播放列表移除
extern "C" FFMPEGAUDIO_API int remove_music(int index);

//销毁播放器
extern "C" FFMPEGAUDIO_API int destroy_player();