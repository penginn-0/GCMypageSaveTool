using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GCMypageSaveTool
{
    internal class GCAPI
    {
        static HttpClient Client = new HttpClient();
        /*
         * https://mypage.groovecoaster.jp/sp/login/auth_con.php
         * https://mypage.groovecoaster.jp/sp/json/player_data.php
         * https://mypage.groovecoaster.jp/sp/json/music_detail.php?music_id={Id}
         * https://mypage.groovecoaster.jp/sp/json/music_list.php"
         */
        private const int APIIntervalMS = 10000;
        private static readonly string LoginEndPoint = "https://mypage.groovecoaster.jp/sp/login/auth_con.php";
        private static readonly string PlayerDataEndPoint = "https://mypage.groovecoaster.jp/sp/json/player_data.php";
        private static readonly string MusicDetailEndPoint = "https://mypage.groovecoaster.jp/sp/json/music_detail.php?music_id=";
        private static readonly string MusicListEndPoint = "https://mypage.groovecoaster.jp/sp/json/music_list.php";
        /// <summary>
        /// ログインしてプレイヤーデータを取得する
        /// エラーならばfalse
        /// </summary>
        /// <param name="API">エンドポイント</param>
        /// <param name="Json"></param>
        /// <returns></returns>
        public bool LoginAndCheck(Credential Cred)
        {
            var parameters = new Dictionary<string, string>(){{ "nesicaCardId",Cred.CardID  },{ "password",Cred.Password },};
            var content = new FormUrlEncodedContent(parameters);
            try
            {
                Console.WriteLine("ログイン中...");
                var Login = Task.Run(()=> Client.PostAsync(LoginEndPoint, content));
                Login.Wait();
                var ResponeText = Client.GetAsync(PlayerDataEndPoint).Result.Content.ReadAsStringAsync();
                var PlayerData = JsonSerializer.Deserialize<PlayerData_Rootobject>(ResponeText.Result);
                if (PlayerData.status != 0)
                {
                    Console.WriteLine("ログインに失敗しました。");
                    return false;
                }
                Console.WriteLine("ログインに成功しました。");
                Console.WriteLine($"プレイヤーネーム：{PlayerData.player_data.player_name}");
                StreamWriter sw = new StreamWriter(@"PlayerData.tsv", false, Encoding.UTF8);
                sw.WriteLine("プレイヤーネーム\tトータルスコア\tプレイ済み楽曲数\t順位\tレベル\tアバター\t称号\tトロフィー数\tトロフィー順位\t平均スコア\tバージョン");
                sw.WriteLine($"{PlayerData.player_data.player_name}\t{PlayerData.player_data.total_score}\t{PlayerData.player_data.total_play_music}/{PlayerData.player_data.total_music}\t{PlayerData.player_data.rank}\t{PlayerData.player_data.level}\t{PlayerData.player_data.avatar}\t{PlayerData.player_data.title}\t{PlayerData.player_data.total_trophy}\t{PlayerData.player_data.trophy_rank}\t{PlayerData.player_data.average_score}\t{PlayerData.player_data.version}");
                sw.WriteLine("クリアステージ数\tノーミスステージ数\tフルチェインステージ数\tパーフェクトステージ数\tSステージ数\tS+ステージ数\tS++ステージ数");
                sw.WriteLine($"{PlayerData.stage.clear}/{PlayerData.stage.all}\t{PlayerData.stage.nomiss}/{PlayerData.stage.all}\t{PlayerData.stage.fullchain}/{PlayerData.stage.all}\t{PlayerData.stage.perfect}/{PlayerData.stage.all}\t{PlayerData.stage.s}/{PlayerData.stage.all}\t{PlayerData.stage.ss}/{PlayerData.stage.all}\t{PlayerData.stage.sss}/{PlayerData.stage.all}");
                sw.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }
        public void GetPlayedMusics()
        {
            Console.WriteLine("プレイ済み楽曲を取得中...");
            var JsonRet = Client.GetAsync(MusicListEndPoint).Result.Content.ReadAsStringAsync().Result;
            var MusicList = JsonSerializer.Deserialize<MusicList_Rootobject>(JsonRet);
            var TotalPlayedCount = 0;
            foreach (var Music in MusicList.music_list)
            {
                TotalPlayedCount += Music.play_count;
            }
            Console.WriteLine("プレイ済み楽曲を取得しました。");
            Console.WriteLine($"トータルプレイTUNE：{TotalPlayedCount}");
            StreamWriter sw;
            var sb = new StringBuilder();
            sb.AppendLine("楽曲ID\t楽曲名\tカナ\t楽曲プレイ回数\t最終プレイ日時");
            foreach (var Music in MusicList.music_list)
            {
                sb.AppendLine($"{Music.music_id}\t{Music.music_title}\t{Music.kana}\t{Music.play_count}\t{Music.last_play_time}");
            }
            sw = new StreamWriter(@"PlayedMusicList.tsv", false, Encoding.UTF8);
            sw.Write(sb.ToString());
            sw.Dispose();
            sb.Clear();
            sb.AppendLine(
                "楽曲ID\t楽曲名\tアーティスト\t使用スキン\tお気に入り\t" +
                "プレイ回数(S)\tノーミス数(S)\tフルチェイン数(S)\tパーフェクト数(S)\t評価(S)\tスコア(S)\tMAX-CHAIN(S)\tAD-LIB数(S)\t"+
                "プレイ回数(N)\tノーミス数(N)\tフルチェイン数(N)\tパーフェクト数(N)\t評価(N)\tスコア(N)\tMAX-CHAIN(N)\tAD-LIB数(N)\t"+
                "プレイ回数(H)\tノーミス数(H)\tフルチェイン数(H)\tパーフェクト数(H)\t評価(H)\tスコア(H)\tMAX-CHAIN(H)\tAD-LIB数(H)\t"+
                "プレイ回数(EX)\tノーミス数(EX)\tフルチェイン数(EX)\tパーフェクト数(EX)\t評価(EX)\tスコア(EX)\tMAX-CHAIN(EX)\tAD-LIB数(EX)\t"+
                "順位(S)\t順位(N)\t順位(H)\t順位(EX)\t"
                );
            Console.WriteLine("プレイ済み楽曲のデータ取得を行います、サーバ負荷軽減のため低速で取得します。");
            var Count = 0;
            var TotalSimplePlayCount = 0;
            var TotalNormalPlayCount = 0;
            var TotalHardPlayCount = 0;
            var TotalExtraPlayCount = 0;
            var TotalNoMissCount = 0;
            var TotalFullChainCount = 0;
            var TotalPerfectCount = 0;
            var TotalAdlibCount = 0;
            var TotalChainCount = 0;
            foreach (var Music in MusicList.music_list)
            {
                Console.WriteLine($"データ取得中({++Count}/{MusicList.music_list.Length})");
                JsonRet = Client.GetAsync(MusicDetailEndPoint + Music.music_id).Result.Content.ReadAsStringAsync().Result;
                var MusicData = JsonSerializer.Deserialize<MusicDetail_Rootobject>(JsonRet);
                sb.Append($"{MusicData.music_detail.music_id}\t{MusicData.music_detail.music_title}\t{MusicData.music_detail.artist}\t{MusicData.music_detail.skin_name}\t{MusicData.music_detail.fav_flg}\t");
                if (MusicData.music_detail.simple_result_data != null)
                {
                    TotalSimplePlayCount += MusicData.music_detail.simple_result_data.play_count;
                    TotalNoMissCount += MusicData.music_detail.simple_result_data.no_miss;
                    TotalFullChainCount += MusicData.music_detail.simple_result_data.full_chain;
                    TotalPerfectCount += MusicData.music_detail.simple_result_data.perfect;
                    TotalAdlibCount += MusicData.music_detail.simple_result_data.adlib;
                    TotalChainCount += MusicData.music_detail.simple_result_data.max_chain;
                    sb.Append(
                        $"{MusicData.music_detail.simple_result_data.play_count}\t" +
                        $"{MusicData.music_detail.simple_result_data.no_miss}\t" +
                        $"{MusicData.music_detail.simple_result_data.full_chain}\t" +
                        $"{MusicData.music_detail.simple_result_data.perfect}\t" +
                        $"{MusicData.music_detail.simple_result_data.rating}\t" +
                        $"{MusicData.music_detail.simple_result_data.score}\t" +
                        $"{MusicData.music_detail.simple_result_data.max_chain}\t" +
                        $"{MusicData.music_detail.simple_result_data.adlib}\t"
                        );
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t\t\t");
                }
                if (MusicData.music_detail.normal_result_data != null)
                {
                    TotalNormalPlayCount += MusicData.music_detail.normal_result_data.play_count; TotalNoMissCount += MusicData.music_detail.normal_result_data.no_miss;
                    TotalFullChainCount += MusicData.music_detail.normal_result_data.full_chain;
                    TotalPerfectCount += MusicData.music_detail.normal_result_data.perfect;
                    TotalAdlibCount += MusicData.music_detail.normal_result_data.adlib;
                    TotalChainCount += MusicData.music_detail.normal_result_data.max_chain;
                    sb.Append(
                        $"{MusicData.music_detail.normal_result_data.play_count}\t" +
                        $"{MusicData.music_detail.normal_result_data.no_miss}\t" +
                        $"{MusicData.music_detail.normal_result_data.full_chain}\t" +
                        $"{MusicData.music_detail.normal_result_data.perfect}\t" +
                        $"{MusicData.music_detail.normal_result_data.rating}\t" +
                        $"{MusicData.music_detail.normal_result_data.score}\t" +
                        $"{MusicData.music_detail.normal_result_data.max_chain}\t" +
                        $"{MusicData.music_detail.normal_result_data.adlib}\t"
                        );
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t\t\t");
                }
                if (MusicData.music_detail.hard_result_data != null)
                {
                    TotalHardPlayCount += MusicData.music_detail.hard_result_data.play_count;
                    TotalNoMissCount += MusicData.music_detail.hard_result_data.no_miss;
                    TotalFullChainCount += MusicData.music_detail.hard_result_data.full_chain;
                    TotalPerfectCount += MusicData.music_detail.hard_result_data.perfect;
                    TotalAdlibCount += MusicData.music_detail.hard_result_data.adlib;
                    TotalChainCount += MusicData.music_detail.hard_result_data.max_chain;
                    sb.Append(
                        $"{MusicData.music_detail.hard_result_data.play_count}\t" +
                        $"{MusicData.music_detail.hard_result_data.no_miss}\t" +
                        $"{MusicData.music_detail.hard_result_data.full_chain}\t" +
                        $"{MusicData.music_detail.hard_result_data.perfect}\t" +
                        $"{MusicData.music_detail.hard_result_data.rating}\t" +
                        $"{MusicData.music_detail.hard_result_data.score}\t" +
                        $"{MusicData.music_detail.hard_result_data.max_chain}\t" +
                        $"{MusicData.music_detail.hard_result_data.adlib}\t"
                        );
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t\t\t");
                }
                if (MusicData.music_detail.extra_result_data != null)
                {
                    TotalExtraPlayCount += MusicData.music_detail.extra_result_data.play_count;
                    TotalNoMissCount += MusicData.music_detail.extra_result_data.no_miss;
                    TotalFullChainCount += MusicData.music_detail.extra_result_data.full_chain;
                    TotalPerfectCount += MusicData.music_detail.extra_result_data.perfect;
                    TotalAdlibCount += MusicData.music_detail.extra_result_data.adlib;
                    TotalChainCount += MusicData.music_detail.extra_result_data.max_chain;
                    sb.Append(
                        $"{MusicData.music_detail.extra_result_data.play_count}\t" +
                        $"{MusicData.music_detail.extra_result_data.no_miss}\t" +
                        $"{MusicData.music_detail.extra_result_data.full_chain}\t" +
                        $"{MusicData.music_detail.extra_result_data.perfect}\t" +
                        $"{MusicData.music_detail.extra_result_data.rating}\t" +
                        $"{MusicData.music_detail.extra_result_data.score}\t" +
                        $"{MusicData.music_detail.extra_result_data.max_chain}\t" +
                        $"{MusicData.music_detail.extra_result_data.adlib}\t"
                        );
                }
                else
                {
                    sb.Append("\t\t\t\t\t\t\t\t");
                }

                var Ranks = new List<User_Rank>();
                foreach (var rank in MusicData.music_detail.user_rank)
                {
                    if (rank is null)
                    { continue; }
                    Ranks.Add(rank);
                }
                Ranks.Sort((a, b) => a.difficulty - b.difficulty);
                if (Ranks.Count < 3 && MusicData.music_detail.ex_flag == 0)
                {
                    var i = -1;
                    foreach (var rank in Ranks)
                    {
                        i++;
                        if (i == 0)
                        {
                            if (rank.difficulty == 0)
                            {
                                sb.Append($"{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 1)
                            {
                                sb.Append($"\t{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 2)
                            {
                                sb.Append($"\t\t{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"\t\t\t{rank.rank}");
                            }
                        }
                        else if (i == 1)
                        {
                            if (rank.difficulty == 1)
                            {
                                sb.Append($"{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 2)
                            {
                                sb.Append($"\t{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"\t\t{rank.rank}");
                            }
                        }
                        else if (i == 2)
                        {
                            if (rank.difficulty == 2)
                            {
                                sb.Append($"{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"\t{rank.rank}");
                            }
                        }
                        else if (i == 3)
                        {
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"{rank.rank}");
                            }
                        }
                    }
                }
                else if (Ranks.Count < 4 && MusicData.music_detail.ex_flag == 1)
                {
                    var i = -1;
                    foreach (var rank in Ranks)
                    {
                        i++;
                        if (i == 0)
                        {
                            if (rank.difficulty == 0)
                            {
                                sb.Append($"{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 1)
                            {
                                sb.Append($"\t{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 2)
                            {
                                sb.Append($"\t\t{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"\t\t\t{rank.rank}");
                            }
                        }
                        else if (i == 1)
                        {
                            if (rank.difficulty == 1)
                            {
                                sb.Append($"{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 2)
                            {
                                sb.Append($"\t{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"\t\t{rank.rank}");
                            }
                        }
                        else if (i == 2)
                        {
                            if (rank.difficulty == 2)
                            {
                                sb.Append($"{rank.rank}\t");
                                continue;
                            }
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"\t{rank.rank}");
                            }
                        }
                        else if (i == 3)
                        {
                            if (rank.difficulty == 3)
                            {
                                sb.Append($"{rank.rank}");
                            }
                        }
                    }
                }
                else
                {
                    foreach (var rank in Ranks)
                    {
                        sb.Append($"{rank.rank}\t");
                    }
                }
                    sb.AppendLine();
                    Console.WriteLine($"データ取得完了({Count}/{MusicList.music_list.Length})");
#if DEBUG
                if(Count ==5)
                {
                    break;
                }
#endif
                    Task.Delay(APIIntervalMS).Wait();
                }
            
            sw = new StreamWriter(@"PlayedMusicDetail.tsv", false, Encoding.UTF8);
            sw.Write(sb.ToString());
            sw.Dispose();
            sw = new StreamWriter(@"PlayerData.tsv", true, Encoding.UTF8);
            sw.WriteLine("トータルプレイTUNE\tプレイ回数(S)\tプレイ回数(N)\tプレイ回数(H)\tプレイ回数(EX)");
            sw.WriteLine($"{TotalPlayedCount}\t{TotalSimplePlayCount}\t{TotalNormalPlayCount}\t{TotalHardPlayCount}\t{TotalExtraPlayCount}");
            sw.WriteLine("トータルNO MISS数\tトータルFULL CHAIN数\tトータルPERFECT数");
            sw.WriteLine($"{TotalNoMissCount}\t{TotalFullChainCount}\t{TotalPerfectCount}");
            sw.WriteLine("トータルAD-LIB数\tトータルMAX-CHAIN数");
            sw.WriteLine($"{TotalAdlibCount}\t{TotalChainCount}");
            sw.Dispose();
            Console.WriteLine("プレイ済み楽曲データ取得が完了しました");

        }
    }
}
