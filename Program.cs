using System;
using System.IO;
using System.Text;

namespace CppHeaderComment
{
    class Program
    {
        static void Main(string[] args)
        {
            AddCppHeaderComment(Console.ReadLine(), "akr", "yyyy-MM-dd");
        }

        static void AddCppHeaderComment(string path, string author, string dateFormat)
        {
            foreach (var cppHeader in Directory.EnumerateFiles(path))
            {
                if (Path.GetExtension(cppHeader) == ".hh")
                {
                    var tempFile = Path.GetTempFileName();

                    using (var tempFileStream = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite))
                    {
                        using (var fileStream = new FileStream(cppHeader, FileMode.Open, FileAccess.ReadWrite))
                        {
                            byte[] buffer = new byte[2];

                            fileStream.Read(buffer, 0, 2);

                            if (buffer[0] == '/' && (buffer[1] == '/' || buffer[2] == '*'))
                            {
                                return;
                            }

                            var cppHeaderComment =
                                      $"// @header: {Path.GetFileName(cppHeader)}\n"
                                    + $"// @author: {author}\n"
                                    + $"// @create: {File.GetCreationTime(cppHeader).ToString(dateFormat)}\n"
                                    + "\n";

                            buffer = Encoding.UTF8.GetBytes(cppHeaderComment);

                            tempFileStream.Write(buffer, 0, buffer.Length);

                            fileStream.Seek(0, SeekOrigin.Begin);

                            for (var r = fileStream.ReadByte(); r != -1; r = fileStream.ReadByte())
                            {
                                tempFileStream.WriteByte((byte)r);
                            }
                        }
                    }

                    File.Copy(tempFile, cppHeader, true);

                    File.Delete(tempFile);
                }
            }
        }
    }
}
