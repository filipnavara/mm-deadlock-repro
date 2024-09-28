using Android.Media;

namespace marshal2;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);       
    }

    protected override void OnStart()
    {
        base.OnStart();

        var mp = new MediaPlayer();
        var s = typeof(MainActivity).Assembly.GetManifestResourceStream("marshal2.ukulele.mp3");
        mp.SetDataSource(new StreamMediaDataSource(s));
        mp.Prepare();
    }

    class StreamMediaDataSource(System.IO.Stream data) : MediaDataSource
    {
        public override long Size => data.Length;

        public override int ReadAt(long position, byte[]? buffer, int offset, int size)
        {
            try
            {
                Console.WriteLine($"XXX StreamMediaDataSource.ReadAt {position} {buffer} {buffer?.Length ?? 0} {offset} {size}");

                // Allocate enough to trigger GC
                for (int i = 0; i < 1000; i++)
                    _ = new byte[8192];

                if (data.CanSeek)
                    data.Seek(position, SeekOrigin.Begin);
                return data.Read(buffer ?? [], offset, size);
            }
            finally
            {
                Console.WriteLine($"XXX //StreamMediaDataSource.ReadAt {position} {buffer} {buffer?.Length ?? 0} {offset} {size}");
            }
        }

        public override void Close()
        {
            data.Dispose();
            data = System.IO.Stream.Null;
        }
    }
}