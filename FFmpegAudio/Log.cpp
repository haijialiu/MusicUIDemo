#include "pch.h"
#include "Log.h"
#include <iostream>

void Log::init()
{
    std::cout << "初始化日志中..." << std::endl;

    //auto sink = logging::add_file_log(
    //    keywords::file_name = "%Y%m%d_%4N.log",
    //    keywords::rotation_size = 5 * 1024 * 1024,
    //    keywords::time_based_rotation =
    //    sinks::file::rotation_at_time_point(23, 59, 59),
    //    keywords::format = (
    //        expr::stream << expr::format_date_time<boost::posix_time::ptime>("TimeStamp",
    //            "%Y-%m-%d %H:%M:%S") << " <" << logging::trivial::severity <<
    //        "> " << expr::smessage
    //        )
    //);
    //sink->locked_backend()->set_file_collector(
    //    sinks::file::make_collector(
    //        keywords::target = "./logs",
    //        keywords::max_size = 500 * 1024 * 1024,
    //        keywords::min_free_space = 800 * 1024 * 1024
    //    )
    //);

    //sink->locked_backend()->auto_flush(true);
    //sink->locked_backend()->scan_for_files();
    // 
    logging::core::get()->set_filter(
        logging::trivial::severity >= logging::trivial::error
    );
    std::cout << "初始化完成！" << std::endl;
}
void Log::destroy()
{
    logging::core::get()->remove_all_sinks();
}

void Log::debug(const std::string& msg)
{
    src::severity_logger_mt<logging::trivial::severity_level> lg;
    BOOST_LOG_SEV(lg, logging::trivial::debug) << msg;
}

void Log::info(const std::string& msg)
{
    src::severity_logger_mt<logging::trivial::severity_level> lg;
    BOOST_LOG_SEV(lg, logging::trivial::info) << msg;
}

void Log::error(const std::string& msg)
{
    src::severity_logger_mt<logging::trivial::severity_level> lg;
    BOOST_LOG_SEV(lg, logging::trivial::error) << msg;
}



