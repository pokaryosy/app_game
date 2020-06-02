using Sequence = System.Collections.IEnumerator;

/// <summary>
/// ゲームクラス。
/// </summary>
public sealed class Game : GameBase
{
    // 変数の宣言
    //int camera_id;
    //string camera_name;

    string pname = "7777";
    string url = "";
    string str = "";

    int gameState = 0;
    int count = 0;
    int high_score = 0;
    int score = 0;

    int tate = 1290;
    int yoko = 726;

    int back_img = 0;

    float key_x = 800;
    float key_y = 1200;
    float key_speed = 20.0f;

    //GPS変数
    float lat;
    float lng;
    string text;
    bool isStartGPS = false;
    float base_lat = 0, base_lng = 0;
    float player_lat = 0, player_lng = 0;
    const int CHECK_NUM = 9;
    int[] check_dx = new int[CHECK_NUM];
    int[] check_dy = new int[CHECK_NUM];
    bool[] isCheck = new bool[CHECK_NUM];
    bool isComplete;
    float calcRate = 0.001f;


    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame()
    {
        resetValue();
    }

    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame()
    {

        if (gameState == 0)
        {
            //タイトル画面の処理
            if (gc.GetPointerFrameCount(0) == 1)
            {
                gameState = 1;
                gc.PlaySound(0, false);
                count++;
                score = count / 60;
            }
            high_score = gc.Load(0);

        }
        else if (gameState == 1)
        {
            //ゲーム中の処理
            back_img = 1;

            /*
            if (gc.GetPointerFrameCount(0) == 1)
            {
                gc.PlaySound(2, false);
            }
            */

            if (gc.GetPointerFrameCount(0) > 3)
            {
                gc.PlaySound(1, false);
                gameState = 2;
            }


        }
        else if (gameState == 2)
        {
            //ゲーム中の処理
            back_img = 0;

            if (gc.GetPointerDuration(0) >= 2.0f)
            {
                gc.PlaySound(1, false);
                gameState = 3;
            }

        }
        else if (gameState == 3)
        {
            //ゲーム中の処理
            back_img = 1;

            key_x += gc.AccelerationLastX * key_speed;
            key_y -= gc.AccelerationLastY * key_speed;

            if (gc.CheckHitRect((int)key_x, (int)key_y, 120, 60, 150, 650, 60, 60))
            //if(gc.CheckHitRect(key_x[i], key_y[i]、幅、高さ、プレイヤーの座標、幅、高さ))
            {
                gc.PlaySound(1, false);
                gameState = 4;
            }

        }
        else if (gameState == 4)
        {
            //ゲーム中の処理
            back_img = 0;
            gc.PlaySound(3, false);
            //判定の部分

            player_lat = lat - base_lat;
            player_lng = lng - base_lng;

            for (int i = 0; i < CHECK_NUM; i++)
            {
                float check_lat = check_dx[i] * calcRate;
                float check_lng = check_dy[i] * calcRate;

                if (
    player_lat - check_lat > -calcRate / 2 &&
    player_lat - check_lat < calcRate / 2 &&
    player_lng - check_lng > -calcRate / 2 &&
    player_lng - check_lng < calcRate / 2
      )
                {
                    isCheck[i] = true;
                }
            }

            //全部通ったかの判定
            isComplete = true;
            for (int i = 0; i < CHECK_NUM; i++)
            {
                if (!isCheck[i])
                {
                    isComplete = false;
                }
            }
            if (isComplete)
            {
                gameState = 5;
                gc.Save(0, high_score);
                gc.PlaySound(1, false);
                if (score > high_score)
                {
                    high_score = score;
                }
            }


        }
        else if (gameState == 5)
        {
            //クリア時の処理
            if (gc.GetPointerFrameCount(0) == 1)
            {
                url = "http://web.sfc.keio.ac.jp/~wadari/sdp/k07_web/score.cgi?score="
                  + score + "&name=" + pname;
                gc.GetOnlineTextAsync(url, out str);
            }
            //長押しでリセット
            if (gc.GetPointerDuration(0) >= 2.0f)
            {
                resetValue();
            }
        }





    }

    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame()
    {
        gc.ClearScreen();

        gc.SetColor(0, 0, 0);
        gc.SetFontSize(35);



        if (gameState == 0)
        {
            //タイトル画面の処理
            gc.DrawImage(back_img, 0, 0);
            gc.SetFontSize(60);
            gc.DrawString("と", 333, 400);
            gc.DrawString("び", 333, 650);
            gc.DrawString("ら", 333, 900);


        }
        else if (gameState == 1)
        {
            //ゲーム中の処理
            gc.ClearScreen();
            gc.DrawImage(back_img, 0, 0);
            gc.DrawString("knock", 150, 750);

        }
        else if (gameState == 2)
        {
            //ゲーム中の処理
            gc.ClearScreen();
            gc.DrawImage(back_img, 0, 0);
            gc.SetFontSize(20);
            gc.DrawString("push", 150, 750);

        }
        else if (gameState == 3)
        {
            //ゲーム中の処理
            gc.ClearScreen();
            gc.DrawImage(back_img, 0, 0);

            gc.DrawImage(2, (int)key_x, (int)key_y);

        }
        else if (gameState == 4)
        {
            //ゲーム中の処理
            gc.ClearScreen();
            gc.DrawImage(back_img, 0, 0);


            for (int i = 0; i < CHECK_NUM; i++)
            {
                gc.SetFontSize(60);
                if (isCheck[i])
                {
                    gc.DrawString("o", 333 + check_dx[i] * 40, 650 + check_dy[i] * 40);
                }
                else
                {
                    gc.DrawString("x", 333 + check_dx[i] * 40, 650 + check_dy[i] * 40);
                }
            }

        }
        else if (gameState == 5)
        {
            //クリア時の処理
            gc.ClearScreen();
            gc.SetFontSize(60);
            gc.DrawString("Congratulations!!", 60, 120);
            gc.SetFontSize(30);
            gc.DrawString("HighScore  Score", 60, 300);
            gc.DrawString(str, 60, 350);

            gc.DrawString("MY HIGH:" + high_score, 60, 400);
            gc.SetFontSize(25);
            gc.DrawString("HighScore&Score(touch)", 60, 450);
            gc.DrawString("RESTART(touch2sec)", 60, 475);
        }


    }


    void resetValue()
    {
        gc.ClearScreen();
        gc.SetResolution(yoko, tate);
        back_img = 0;

        score = 0;
        count = 0;
        gameState = 0;

        for (int i = 0; i < CHECK_NUM; i++)
        {
            isCheck[i] = false;
            check_dx[i] = (i % 3) - 1;
            check_dy[i] = (i / 3) - 1;
        }
        isComplete = false;

    }


}
