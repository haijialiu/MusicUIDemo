#pragma once
#include <boost/log/core.hpp>
#include <boost/log/expressions.hpp>
#include <boost/log/sinks/sync_frontend.hpp>
#include <boost/log/sinks/text_ostream_backend.hpp>
#include <boost/log/sources/record_ostream.hpp>
#include <boost/log/sources/severity_logger.hpp>
#include <boost/log/support/date_time.hpp>
#include <boost/log/utility/setup/file.hpp>
#include <boost/log/utility/setup/common_attributes.hpp>
#include <boost/log/trivial.hpp>
#include <string>
namespace logging = boost::log;
namespace sinks = boost::log::sinks;
namespace src = boost::log::sources;
namespace expr = boost::log::expressions;
namespace attrs = boost::log::attributes;
namespace keywords = boost::log::keywords;

class Log
{
public:
	static void init();
	static void destroy();
	static void debug(const std::string& msg);
	static void info(const std::string& msg);
	static void error(const std::string& msg);
private:
	Log();
	~Log() {}
	Log(const Log&);
	const Log& operator=(const Log&) = delete;

	static Log* g_pLog;
};

