#pragma once
#include<exception>
#include <optional>
#include <queue>
#include <memory>
#include "common.h"

namespace media {
	using std::optional, std::nullopt;

	template<typename T>
	class MediaQueue
	{
	public:
		bool empty()
		{
			return this->_queue.empty();
		}
		int push(T value)
		{
			std::lock_guard<std::mutex> lock(_mutex);
			if (this->_abort)
			{
				//Log::error("插入失败，序列状态为终止！");
				return -1;
			}
			this->_queue.push(value);

			this->_cond.notify_one();
			return 0;
		}
		optional<T> front()
		{
			std::lock_guard<std::mutex> lock(_mutex);
			if (this->_abort)
			{
				//Log::error("队列已终止！");
				throw "队列已终止！";
			}
			if (!_abort && !_queue.empty())
			{
				return _queue.front();
			}
			else
			{
				//Log::error("front：队列已终止 或为空");
				return nullopt;
			}

			return this->_queue.front();
		}
		optional<T> pop(const int timeout = 0)
		{
			std::unique_lock<std::mutex> lock(_mutex);
			//std::lock_guard<std::mutex> lock(_mutex);
			if (!_queue.empty())
			{
				T ret = std::move(_queue.front());
				_queue.pop();
				return ret;
			}
			else 
			{
				if (_queue.empty()) 
				{
					//未明白
					
					_cond.wait_for(lock, std::chrono::milliseconds(timeout), [this] {
						return !_queue.empty() | (_abort == 1);
					});
				}
				return nullopt;
			}
			//lock.unlock();
		}
		/*int pop(const int timeout = 0)
		{
			std::unique_lock<std::mutex> lock(_mutex);
			if (this->_queue.empty())
			{
				this->_cond.wait_for(lock, std::chrono::microseconds(timeout), [this] {
					return !_queue.empty() | _abort;
					});
			}
			if (this->_abort == 1)
			{
				return -1;
			}
			if (this->_queue.empty())
			{
				return -2;
			}
			this->_queue.pop();
			return 0;
		}*/
		int size() { return this->_queue.size(); }

		void abort()
		{
			this.release();
			this->_abort = 1;
			this->_cond.notify_all();
		}
	private:
		void release()
		{
			//Log::debug("请根据需要判断是否需要实现序列释放");
		}

		int _abort = 0;
		std::mutex _mutex;
		std::condition_variable _cond;
		std::queue<T> _queue;

	};

	template<>
	inline int MediaQueue<AVPacket*>::push(AVPacket* value)
	{
		
		std::lock_guard<std::mutex> lock(_mutex);
		if (this->_abort)
		{
			//Log::error("插入失败，序列状态为终止！");
			return -1;
		}
		//深拷贝避免释放
		//TODO: 被定位到有内存泄露，请检查
		AVPacket* tmp_pkt = av_packet_alloc();
		av_packet_move_ref(tmp_pkt, value);
		this->_queue.push(tmp_pkt);
		av_packet_unref(value);
		this->_cond.notify_one();
		return 0;
	}

	template<>
	inline int MediaQueue<AVFrame*>::push(AVFrame* value)
	{
		std::lock_guard<std::mutex> lock(_mutex);
		if (this->_abort)
		{
			//Log::error("插入失败，序列状态为终止！");
			return -1;
		}
		//深拷贝避免释放
		AVFrame* tmp_frame = av_frame_alloc();
		av_frame_move_ref(tmp_frame, value);
		this->_queue.push(tmp_frame);
		av_frame_unref(value);
		this->_cond.notify_one();
		return 0;
	}
	template<>
	inline void MediaQueue<AVPacket*>::release()
	{
		//TODO 清空序列资源
		while (!_queue.empty())
		{
			AVPacket* pkt = nullptr;

			pkt = this->_queue.front();
			av_packet_free(&pkt);
		}
	}
	template<>
	inline void MediaQueue<AVFrame*>::release()
	{
		//TODO 清空序列资源
		while (!_queue.empty())
		{
			AVFrame* frame = nullptr;
			frame = this->_queue.front();
			av_frame_free(&frame);
		}
	}
	using AVPacketQueue = MediaQueue<AVPacket*>;
	using AVFrameQueue = MediaQueue<AVFrame*>;
}

