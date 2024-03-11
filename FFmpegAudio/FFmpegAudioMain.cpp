#include "pch.h"
#include "Player.h"
#include "FFmpegAudioMain.h"

using namespace media;

static Player* player;
int create_player()
{
	player = new Player();
	player->main_loop();
	return 0;
}

int operate(const char* action, const char* value)
{
	player->operate(std::string(action), std::string(value));
	return 0;
}

bool share_play_status()
{
	return player->share_current_status();
}

double share_play_time()
{
	if (player->share_current_play_time())
	{
		//std::cout << *player->share_current_play_time() << std::endl;
		return *player->share_current_play_time();
	}
	return 0;
}

int share_play_index()
{
	if(player->share_current_index())
		return *player->share_current_index();
	return 0;
}

int input_music_urls(const char** music_urls, int num)
{
	 using std::string, std::vector;
	 vector<string>_music_urls;
	 for (int i = 0; i < num; i++)
	 {
		 _music_urls.push_back(music_urls[i]);
	 }
	 player->set_play_list(_music_urls);
	 return 0;
}

int remove_music(int index)
{

	return player->operate("remove",std::to_string(index));
}

int destroy_player()
{
	delete player;
	return 0;
}
