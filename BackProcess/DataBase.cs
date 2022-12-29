using System;
using System.Data.SQLite;

namespace ConsoleApp1
{
    class DateBase
    {
        //DB接続用のメンバー
        static private SQLiteConnectionStringBuilder sqlConnectionSb;
        static private SQLiteConnection cn;
        static private SQLiteCommand cmd;
        //テーブルにて作成↓データ数
        static private int[] datecount= {0,0,0,0,0};
        //作成するテーブル名を格納↓配列
        public readonly string[] dbfingername = { "little_finger", "ring_finger", "middle_finger", "index_finger", "thumb_finger" };
        
        //コンストラクタ
        public DateBase()
        {
            try
            {
                sqlConnectionSb = new SQLiteConnectionStringBuilder { DataSource = "denco.db" };
                cn = new SQLiteConnection(sqlConnectionSb.ToString());
                cn.Open();
                cmd = new SQLiteCommand(cn);
            } catch (Exception) {
                Console.WriteLine("接続エラー");
            } 
        }

        //DB切断
        public void Close()
        {
            cn.Close();
            cn.Dispose();
        }

        //テーブルの作成
        //引数：テーブル番号
        public void TableCreate(int _fingernum)
        {
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + dbfingername[_fingernum] + " (" +
                    "no INTEGER NOT NULL PRIMARY KEY," +
                    "trajectory REAL NOT NULL,"+
                    "tipreactionforce INTEGER NOT NULL," +
                    "palmreactionforce INTEGER NOT NULL)";
            cmd.ExecuteNonQuery();
            return;
        }

        //データの追加
        //引数：テーブル番号,軌跡,指先の反力,手のひらの反力
        public void DataInsert(int _fingernum, float _trajectory, int _tipreactionforce,int _palmreactionforce)
        {
            datecount[_fingernum]++;
            cmd.CommandText = "INSERT OR IGNORE INTO " + dbfingername[_fingernum] + " (no, trajectory,tipreactionforce,palmreactionforce) VALUES (" +
                $"{datecount[_fingernum]},{_trajectory},{_tipreactionforce},{_palmreactionforce})";
            cmd.ExecuteNonQuery();
            return;
        }

        //データ検索
        //引数；テーブル番号,軌跡
        //戻り値：int型配列,配列[0]指先の反力,配列[1]手のひらの反力
        public int[] DataIndex(int _fingernum, float _trajectory)
        {
            int[] resultnum=new int[] {0,0};
            string[] resultvalue= new string[]{"",""};
            cmd.CommandText = "SELECT * FROM " + dbfingername[_fingernum] + " WHERE trajectory >= "+ _trajectory+ " ORDER BY trajectory ASC LIMIT 1";
            var reader = cmd.ExecuteReader();
            if (reader.Read()==false)
            {
                reader.Close();
                cmd.CommandText = "SELECT * FROM " + dbfingername[_fingernum] + " WHERE trajectory < " + _trajectory + " ORDER BY trajectory ASC LIMIT 1";
                reader = cmd.ExecuteReader();
                //データが存在しない場合0,0を返す
                if (reader.Read() == false)
                {
                    return resultnum;
                }
            }
            resultvalue[0] = reader["tipreactionforce"].ToString();
            resultnum[0]= int.Parse(resultvalue[0]);
            resultvalue[1] = reader["palmreactionforce"].ToString();
            resultnum[1] = int.Parse(resultvalue[1]);
            reader.Close();
            return resultnum;
        }

        //特定のテーブルデータ削除
        public void DataDelete(int _fingernum)
        {

            cmd.CommandText = "DELETE FROM " + dbfingername[_fingernum];
            cmd.ExecuteNonQuery();
            return;

        }

        //すべてのテーブルデータ削除
        public void DataAllDelete()
        {
            foreach(string deleteindex in dbfingername)
            {
                cmd.CommandText = "DELETE FROM " + deleteindex;
                cmd.ExecuteNonQuery();
            }
            return;

        }

    }
}



/*
static void Main(string[] args)
{
    DateBase datebase = new DateBase();

    try
    {
        for (int i = 0; i < 5; i++)
        {
            datebase.TableCreate(i);
        }

        datebase.DataAllDelete();
        for (int index = 0; index < 10; index++)
        {
            for (int i = 0; i < 5; i++)
            {
                datebase.DataInsert(i, 1.1f * i * index, i, i + 1);
            }
        }

        var value = datebase.DataIndex(1, 4.4f);

        Console.WriteLine("Value1:" + value[0] + "\n");
        Console.WriteLine("Value2:" + value[1] + "\n");
        Console.ReadKey();
    }
    catch (Exception)
    {
        Console.WriteLine("Error!!");
    }
    finally
    {
        datebase.Close();
    }

}
}*/