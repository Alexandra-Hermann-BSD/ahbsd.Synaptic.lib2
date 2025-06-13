using Serilog;

namespace Synaptic.lib2.Tests
{
    public abstract class BaseTest
    {

        private readonly LoggerConfiguration _config = new LoggerConfiguration().WriteTo.File("NeuronTest2.log")
            .MinimumLevel.Information()
            .Enrich.FromLogContext();

        protected BaseTest()
        {
            Log.Logger = _config.CreateLogger();
            Log.Information("[{Name}] Starting test class", Name);
        }

        protected ILogger Logger { get; set; }

        public string Name => GetType().Name;

        public void Deconstruct(out LoggerConfiguration config, out ILogger logger)
        {
            config = _config;
            logger = Logger;
        }

        ~BaseTest()
        {
            Log.Information("[{Name}] Finalizing test class", Name);
            Log.CloseAndFlush();
        }
    }
}