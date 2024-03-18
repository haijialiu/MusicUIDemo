#pragma once

#ifdef MEDIALIBRARY_EXPORTS
#define MEDIALIBRARY_API __declspec(dllexport)
#else
#define MEDIALIBRARY_API __declspec(dllimport)
#endif

typedef struct 
{
	uint32_t image_size;
	uint8_t* image;
} album_info;
typedef struct 
{
	const char* title;
	const char* artist;
	const char* album;
	int total_time;
	album_info rgba_album_info;
} music_info;
extern "C" MEDIALIBRARY_API int media_load(const char* file_url);
extern "C" MEDIALIBRARY_API const char* media_title();
extern "C" MEDIALIBRARY_API const char* media_artist();
extern "C" MEDIALIBRARY_API const char* media_album();
extern "C" MEDIALIBRARY_API int save_media_album(const char* save_path);
extern "C" MEDIALIBRARY_API int media_time();
extern "C" MEDIALIBRARY_API album_info media_album_png();
extern "C" MEDIALIBRARY_API music_info get_music_info();
extern "C" MEDIALIBRARY_API void media_free();


