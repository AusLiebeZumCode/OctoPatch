using System;

namespace OctoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
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
