using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCMypageSaveTool
{
    #region PlayerData
    public class PlayerData_Rootobject
    {
        public int status { get; set; }
        public Player_Data player_data { get; set; }
        public Stage stage { get; set; }
    }

    public class Player_Data
    {
        public string player_name { get; set; }
        public string total_score { get; set; }
        public int total_play_music { get; set; }
        public int total_music { get; set; }
        public int rank { get; set; }
        public int level { get; set; }
        public string avatar { get; set; }
        public string title { get; set; }
        public string total_trophy { get; set; }
        public string trophy_rank { get; set; }
        public int average_score { get; set; }
        public string version { get; set; }
        public bool friendApplication { get; set; }
    }

    public class Stage
    {
        public int all { get; set; }
        public int clear { get; set; }
        public int nomiss { get; set; }
        public int fullchain { get; set; }
        public int perfect { get; set; }
        public int s { get; set; }
        public int ss { get; set; }
        public int sss { get; set; }
    }
    #endregion

    #region MusicList
    public class MusicList_Rootobject
    {
        public int status { get; set; }
        public Music_List[] music_list { get; set; }
    }

    public class Music_List
    {
        public int music_id { get; set; }
        public string music_title { get; set; }
        public string kana { get; set; }
        public int play_count { get; set; }
        public string last_play_time { get; set; }
    }
    #endregion

    #region MusicDetail
    public class MusicDetail_Rootobject
    {
        public int status { get; set; }
        public Music_Detail music_detail { get; set; }
    }

    public class Music_Detail
    {
        public string music_id { get; set; }
        public string music_title { get; set; }
        public string artist { get; set; }
        public string skin_name { get; set; }
        public int ex_flag { get; set; }
        public Simple_Result_Data simple_result_data { get; set; }
        public Normal_Result_Data normal_result_data { get; set; }
        public Hard_Result_Data hard_result_data { get; set; }
        public Extra_Result_Data extra_result_data { get; set; }
        public User_Rank[] user_rank { get; set; }
        public Difficulty[] difficulty { get; set; }
        public int fav_flg { get; set; }
        public object message { get; set; }
    }

    public class Simple_Result_Data
    {
        public string rating { get; set; }
        public int no_miss { get; set; }
        public int full_chain { get; set; }
        public int perfect { get; set; }
        public int play_count { get; set; }
        public bool is_clear_mark { get; set; }
        public bool is_failed_mark { get; set; }
        public string music_level { get; set; }
        public int score { get; set; }
        public int max_chain { get; set; }
        public int adlib { get; set; }
    }

    public class Normal_Result_Data
    {
        public string rating { get; set; }
        public int no_miss { get; set; }
        public int full_chain { get; set; }
        public int perfect { get; set; }
        public int play_count { get; set; }
        public bool is_clear_mark { get; set; }
        public bool is_failed_mark { get; set; }
        public string music_level { get; set; }
        public int score { get; set; }
        public int max_chain { get; set; }
        public int adlib { get; set; }
    }

    public class Hard_Result_Data
    {
        public string rating { get; set; }
        public int no_miss { get; set; }
        public int full_chain { get; set; }
        public int perfect { get; set; }
        public int play_count { get; set; }
        public bool is_clear_mark { get; set; }
        public bool is_failed_mark { get; set; }
        public string music_level { get; set; }
        public int score { get; set; }
        public int max_chain { get; set; }
        public int adlib { get; set; }
    }

    public class Extra_Result_Data
    {
        public string rating { get; set; }
        public int no_miss { get; set; }
        public int full_chain { get; set; }
        public int perfect { get; set; }
        public int play_count { get; set; }
        public bool is_clear_mark { get; set; }
        public bool is_failed_mark { get; set; }
        public string music_level { get; set; }
        public int score { get; set; }
        public int max_chain { get; set; }
        public int adlib { get; set; }
    }

    public class User_Rank
    {
        public int rank { get; set; }
        public int difficulty { get; set; }
    }

    public class Difficulty
    {
        public int music_difficulty { get; set; }
    }
    #endregion
    #region MusicRankingDetail

    public class MusicRankingDetail_Rootobject
    {
        public int status { get; set; }
        public string count { get; set; }
        public Score_Rank[] score_rank { get; set; }
    }

    public class Score_Rank
    {
        public int rank { get; set; }
        public string player_name { get; set; }
        public string event_point { get; set; }
        public string title { get; set; }
        public string last_play_tenpo_name { get; set; }
        public string pref { get; set; }
    }

    #endregion
}
