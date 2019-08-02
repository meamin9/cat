using Microsoft.VisualStudio.TestTools.UnitTesting;
using AM.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AM.Game.Tests
{
    class A
    {
        byte a;
    }

    [TestClass()]
    public class MainTests
    {

        [TestMethod()]
        public void CreateInstanceTest()
        {
            var times = 512*1024;
            var memSize = 8 * 1024 * 1024;
            var watch = new Stopwatch();
            System.Func<A> aLambda;
            Type aType;

            TimeSpan average = TimeSpan.Zero,max= TimeSpan.MinValue, min = TimeSpan.MaxValue;

            bool ok;
            var count = 10;
            for (var j = 0; j < count; ++j) {
                ok = GC.TryStartNoGCRegion(memSize);
                if (!ok) {
                    Assert.Fail("not enough memery");
                    return;
                }
                watch.Start();
                for (var i = 0; i < times; ++i) {
                    //alist.Add(new A());
                    new A();
                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed + " (Directly new)");
                average += watch.Elapsed;
                max = max < watch.Elapsed ? watch.Elapsed: max;
                min = min > watch.Elapsed ? watch.Elapsed : min;
                //alist.Clear();
                watch.Reset();
                GC.EndNoGCRegion();
                GC.WaitForFullGCComplete();
            }
            average = new TimeSpan(average.Ticks/count);
            Console.WriteLine($"average={average}, max={max},min={min},range={max - min}");

            average = TimeSpan.Zero;
            max = TimeSpan.MinValue;
            min = TimeSpan.MaxValue;
            for (var j = 0; j < count; ++j) {
                ok = GC.TryStartNoGCRegion(memSize);
                if (!ok) {
                    Assert.Fail("not enough memery");
                    return;
                }
                watch.Start();
                aLambda = () => new A();
                for (var i = 0; i < times; ++i) {
                    aLambda();
                    //alist.Add(aLambda());
                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed + " (lamda new)");
                average += watch.Elapsed;
                max = max < watch.Elapsed ? watch.Elapsed : max;
                min = min > watch.Elapsed ? watch.Elapsed : min;
                watch.Reset();
                GC.EndNoGCRegion();
                GC.WaitForFullGCComplete();
            }
            average = new TimeSpan(average.Ticks / count);
            Console.WriteLine($"average={average}, max={max},min={min},range={max-min}");

            average = TimeSpan.Zero;
            max = TimeSpan.MinValue;
            min = TimeSpan.MaxValue;
            for (var j = 0; j < count; ++j) {
                ok = GC.TryStartNoGCRegion(memSize);
                if (!ok) {
                    Assert.Fail("not enough memery");
                    return;
                }
                watch.Start();
                aType = typeof(A);
                for (var i = 0; i < times; ++i) {
                    Activator.CreateInstance(aType);
                    //alist.Add((A)Activator.CreateInstance(aType));
                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed + " (Activator CreateInstance)");
                average += watch.Elapsed;
                max = max < watch.Elapsed ? watch.Elapsed : max;
                min = min > watch.Elapsed ? watch.Elapsed : min;
                watch.Reset();
                GC.EndNoGCRegion();
                GC.WaitForFullGCComplete();
            }
            average = new TimeSpan(average.Ticks / count);
            Console.WriteLine($"average={average}, max={max},min={min},range={max - min}");
        }
    }
}