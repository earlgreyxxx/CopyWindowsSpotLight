/******************************************************************
FileName : CopyWindowsSpotLight.cs
******************************************************************/
using System;
using System.Text;
using System.IO;
using System.Linq;

namespace org.ptsv.console
{
  class CopySpotlight
  {
    // エントリポイント
    static int Main(string [] args)
    {
      if(args.Length < 2)
      {
        Console.Error.WriteLine("Usage: command <source directory> <destination directory>");
        return -1;
      }

      char[] trimChars = { '\\',' ' };
      var src = args[0].TrimEnd(trimChars);
      var dest = args[1].TrimEnd(trimChars);

      if(!Directory.Exists(src) || !Directory.Exists(dest))
        Console.Error.WriteLine("ディレクトリが存在しません。");

      // コピー元のディレクトリのファイル一覧を取得してフィルタリングしていきます。
      // 最終的に プロパティ s,d を持つ匿名クラスに変換して foreach で回していきます。
      // 本来はtry-catchで囲むべきですが・・・
      var enumerator = Directory
        .EnumerateFiles(src)
        .Where(f => (new FileInfo(f)).Length >= 300 * 1024)
        .Where(IsLandscape)
        .Select(f => new { s = f,d = dest + "\\" + Path.GetFileName(f) + ".jpg"});

      foreach(var set in enumerator)
      {
        try {
          File.Copy(set.s,set.d);
          Console.Error.WriteLine("done copied 1 file.");
        } catch (Exception e) {
          Console.Error.WriteLine("skip copy since file already exists.");
        }
      }

      return 0;
    }

    // 値をスワップする（マイクロソフトのドキュメントからコピペ ）
    // https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/generics/generic-methods
    protected static void Swap<T>(ref T lhs, ref T rhs)
    {
      T temp;
      temp = lhs;
      lhs = rhs;
      rhs = temp;
    }

    // ２バイトから16ビット整数を返す
    protected static int ConvertToInt32(byte first,byte second)
    {
      return (first << 8) | second;
    }

    // JPEGファイルの縦横サイズから 幅が高さより大きいものを選択できるようにする
    // BitConverterってリトルエンディアン(x86系なら殆どコレだと思うけど)しか対応してないみたいですねぇ。。。
    // SOFマーカーから取得できるサイズってビックエンディアンで記録されている？のでひと手間必要。ハマった。
    protected static bool IsLandscape(string f)
    {
      int width = 0,height = 0;
      using (FileStream fs = File.OpenRead(f))
      {
        int bufSize = 64 * 1024; // 64KB
        byte[] content = new byte[bufSize];
        int readLen = 0;
        bool isReadBreak = false;
        while (0 < (readLen = fs.Read(content,0,content.Length)))
        {
          for(int index = 0; index < content.Length; index++)
          {
            if((0xFF == content[index] && 0xC0 == content[index + 1]) || (0xFF == content[index] && 0xC2 == content[index + 1]))
            {
              height = ConvertToInt32(content[index + 5],content[index + 6]); 
              width = ConvertToInt32(content[index + 7],content[index + 8]); 
              isReadBreak = true;
              break;
            }
          }
          if(isReadBreak)
            break;

          Array.Clear(content,0,readLen);
        }
        content = null;
      }
      return width > 0 && height > 0 && width >= height;
    }
  }
}
