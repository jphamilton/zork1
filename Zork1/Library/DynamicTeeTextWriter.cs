using System.Text;

namespace Zork1.Library;

public class DynamicTeeTextWriter(TextWriter console) : TextWriter
{
    private readonly TextWriter _console = console;
    private StreamWriter _logFile;

    public override Encoding Encoding => _console.Encoding;

    public void StartLogging(string path)
    {
        _logFile?.Close();

        _logFile = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }

    public void StopLogging()
    {
        _logFile?.Close();
        _logFile = null;
    }

    public void Bold(string value)
    {
        _console.Write("\x1b[1m");
        _console.WriteLine(value);
        _console.Write("\x1b[0m");
        _logFile?.WriteLine(value);
    }

    public void ScriptWrite(string value)
    {
        _logFile?.WriteLine(value);
    }

    public override void Write(char value)
    {
        _console.Write(value);
        _logFile?.Write(value);
    }

    public override void Write(string value)
    {

        _console.Write(value);
        _logFile?.Write(value);
    }

    public override void WriteLine(string value)
    {
        _console.WriteLine(value);
        _logFile?.WriteLine(value);
    }

    public override void Flush()
    {
        _console.Flush();
        _logFile?.Flush();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _logFile?.Dispose();
        }
        base.Dispose(disposing);
    }
}
