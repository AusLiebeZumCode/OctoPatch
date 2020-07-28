using System;
using System.Collections.Generic;
using System.Linq;
using OctoPatch;
using OctoPatch.Exchange;

namespace OctoConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var repository = new Repository();


            var engine = new Engine(repository);

            var grid = new Grid
            {
                Name = "Testgrid",
                Description = "This is a test grid",
                NodeInstances = new[]
                {
                    new NodeInstance
                    {
                        Guid = Guid.Parse("{7D01A02E-6ADA-4A60-ACCF-8B74D246EACC}"),
                        NodeDescription = Guid.Parse("{8AA1AB11-DB28-4098-9999-13A3A47E8A83}"),
                        Configuration = "{}",
                    }
                }.ToList(),
                WireInstances = new List<WireInstance>()
            };

            engine.Load(grid);


            //var dmxDevice = new DmxDevice();
            //dmxDevice.Configure(new DmxDeviceConfiguration());

            //var dmxInput = (IObserver<DmxMessage>)dmxDevice.Inputs.First();
            //dmxInput.OnNext(new DmxMessage { Address = 2, Value = 255 });

            //var midiDevice = new MidiDevice();
            //midiDevice.Configure(new DeviceConfiguration { DeviceName = "LPD8" });

            //var midiOutput = (IObservable<MidiMessage>)midiDevice.Outputs.First();

            //var midiFilter = new MidiMessageFilter();
            //var midiFilterInput = (IObserver<MidiMessage>)midiFilter.Inputs.First();
            //var midiFilterOutput = (IObservable<MidiMessage>)midiFilter.Outputs.First();
            //midiFilter.Configure(new MidiMessageFilter.Configuration
            //{
            //    MessageType = 3,
            //});
            //midiOutput.Subscribe(midiFilterInput);
            //midiFilterOutput.Subscribe(x =>
            //{
            //    switch (x.Key)
            //    {
            //        case 5:
            //            dmxInput.OnNext(new DmxMessage { Address = 0, Value = x.Value * 2 });
            //            break;
            //        case 6:
            //            dmxInput.OnNext(new DmxMessage { Address = 1, Value = x.Value * 2 });
            //            break;
            //        case 7:
            //            dmxInput.OnNext(new DmxMessage { Address = 2, Value = x.Value * 2 });
            //            break;
            //    }

            //    Console.WriteLine($"Type: {x.MessageType}, Channel: {x.Channel}, Key: {x.Key}, Value: {x.Value}");
            //});

            Console.ReadLine();
            //midiDevice.Dispose();
            //dmxDevice.Dispose();
        }
    }
}
