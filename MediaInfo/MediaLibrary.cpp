#include "pch.h"
#include <map>
#include <string>
#include <algorithm>
#include <fstream>
#include "MediaLibrary.h"
extern "C"
{
#include <libavformat/avformat.h>
#include <libavutil/avutil.h>
#include <libavcodec/avcodec.h>
#include <libavutil/imgutils.h>
#include <libswscale/swscale.h>
}

static std::map<std::string, std::string> _metadata;
static AVFormatContext* ifmt;
static int audio_index = -1,video_index = -1;
static std::map<std::string, std::string> get_metadata();
static album_info info{ 0 };
static music_info _music_info{ 0 };
int media_load(const char* file_url)
{	
	int ret = -1;
	ifmt = avformat_alloc_context();
	if (!ifmt)
	{
		return -1;
	}
	ret = avformat_open_input(&ifmt, file_url, nullptr, nullptr);
	if (ret != 0)
		return -2;
	ret = avformat_find_stream_info(ifmt, nullptr);
	if (ret != 0)
		return -3;
	audio_index = av_find_best_stream(ifmt, AVMEDIA_TYPE_AUDIO, -1, -1, nullptr, 0);
	video_index = av_find_best_stream(ifmt, AVMEDIA_TYPE_VIDEO, -1, -1, nullptr, 0);
	if (audio_index < 0)
		return audio_index;
	_metadata = get_metadata();
	return 0;
}

static const char* find_data(std::string key)
{
	auto data = _metadata.find(key);
	if (data != _metadata.end())
		return (*data).second.c_str();
	return nullptr;
}
const char* media_title()
{
	return find_data("title");
}
const char* media_artist()
{

	return find_data("artist");
}
// 专辑名称
const char* media_album()
{
	return find_data("album");
}
int save_media_album(const char* save_path)
{

	return int();
}
int media_time()
{
	if (audio_index >= 0)
	{
		int64_t duration = ifmt->streams[audio_index]->duration;
		auto time_base = ifmt->streams[audio_index]->time_base;
		return duration * time_base.num / time_base.den;
	}
	return 0;
}



//专辑图片
album_info media_album_png()
{

	using namespace std;

	
	if (video_index < 0)
		return info;
	AVPacket pkt;

	AVFrame* frame, * _frame = nullptr;
	if (!ifmt)
	{
		return info;
	}
	auto codec_ctx = avcodec_alloc_context3(nullptr);
	int _ret = -1;
	while (true)
	{
		_ret = av_read_frame(ifmt, &pkt);
		if (_ret < 0)
			return info;
		if (pkt.stream_index == video_index)
		{
			auto avctx = ifmt->streams[video_index]->codecpar;

			frame = av_frame_alloc();
			int ret = avcodec_parameters_to_context(codec_ctx, avctx);
			if (ret < 0)
				break;
			const AVCodec* codec = avcodec_find_decoder(codec_ctx->codec_id);
			if (!codec)
				break;
			ret = avcodec_open2(codec_ctx, codec, nullptr);
			if (ret < 0)
				break;

			ret = avcodec_send_packet(codec_ctx, &pkt);
			if (ret < 0)
				break;
			while (true)
			{
				//auto target_format = AV_PIX_FMT_RGB565;
				auto target_format = AV_PIX_FMT_RGB24;
				ret = avcodec_receive_frame(codec_ctx, frame);
				if (ret == AVERROR(EAGAIN))
					break;
				if (ret == 0)
				{
					//得到了完整的图像
					int width = frame->width;
					int height = frame->height;

					//auto rgb_frame = frame;
					auto rgb_frame = av_frame_alloc();
					rgb_frame->format = target_format;
					rgb_frame->width = width;
					rgb_frame->height = height;
					av_image_alloc(rgb_frame->data, rgb_frame->linesize, width, height, target_format, 1);
					AVPixelFormat pixFormat;
					switch (frame->format) {
					case AV_PIX_FMT_YUVJ420P:
						pixFormat = AV_PIX_FMT_YUV420P;
						break;
					case AV_PIX_FMT_YUVJ422P:
						pixFormat = AV_PIX_FMT_YUV422P;
						break;
					case AV_PIX_FMT_YUVJ444P:
						pixFormat = AV_PIX_FMT_YUV444P;
						break;
					case AV_PIX_FMT_YUVJ440P:
						pixFormat = AV_PIX_FMT_YUV440P;
						break;
					default:
						pixFormat = (AVPixelFormat)frame->format;
						break;
					}
					SwsContext* sws_ctx = sws_getContext(width, height,
						pixFormat, width, height, target_format, SWS_BICUBIC, nullptr, nullptr, nullptr);
					sws_scale(sws_ctx, frame->data, frame->linesize, 0, height, rgb_frame->data, rgb_frame->linesize);




					//编码保存
					auto codec = avcodec_find_encoder(AV_CODEC_ID_PNG);
					AVCodecContext* ctx = avcodec_alloc_context3(codec);
					ctx->width = width;
					ctx->height = height;
					ctx->time_base = { 1,25 };
					ctx->max_b_frames = 0;
					ctx->thread_count = 1;
					ctx->pix_fmt = *codec->pix_fmts;

					int ret = avcodec_open2(ctx, codec, nullptr);

					ret = avcodec_send_frame(ctx, rgb_frame);

					AVPacket* pkt = av_packet_alloc();
					ret = avcodec_receive_packet(ctx, pkt);
					uint8_t* image_data = (uint8_t*)malloc(pkt->size);
					if (image_data)
						memcpy(image_data, pkt->data, pkt->size);
					info.image = image_data;
					info.image_size = pkt->size;

					ofstream out("imagecpp.png", ios::out | ios::binary);
					out.write((char*)pkt->data,pkt->size);
					out.close();
					




					break;
				}
			}

			av_frame_free(&frame);
		}
		else
		{
			av_packet_unref(&pkt);
		}
	}
	avcodec_close(codec_ctx);
	av_frame_free(&frame);
	return info;
}

music_info get_music_info()
{

	_music_info.title = media_title();
	_music_info.artist = media_artist();
	_music_info.album = media_album();
	_music_info.total_time = media_time();
	_music_info.rgba_album_info = media_album_png();
	return  _music_info;
}
void media_free()
{

	if (info.image)
		free(info.image);
	info.image = nullptr;
	info.image_size = 0;
	_music_info = music_info{ 0 };
	avformat_free_context(ifmt);
}
static std::map<std::string, std::string> get_metadata()
{
	std::map<std::string, std::string> metadata;
	AVDictionaryEntry* tag = nullptr;
	if (audio_index >= 0)
	{
		auto _metadata = ifmt->metadata;
		if (!_metadata)
			_metadata = ifmt->streams[audio_index]->metadata;
		while (tag = av_dict_get(_metadata, "", tag, AV_DICT_IGNORE_SUFFIX))
		{
			std::string key(tag->key), value(tag->value);
			std::transform(key.begin(), key.end(), key.begin(), [](unsigned char c) {return std::tolower(c); });
			//std::transform(value.begin(), value.end(), value.begin(), [](unsigned char c) {return std::tolower(c); });
			metadata.insert({ key,value });
		}
	}
	return metadata;

}
