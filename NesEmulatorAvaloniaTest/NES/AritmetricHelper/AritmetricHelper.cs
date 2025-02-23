using System.Numerics;

namespace NesEmulatorAvaloniaTest.NES.AritmetricHelper;

public static class AritmetricHelper
{
    /*Helper methods to simulate Rusts wrapping add and wrapping sub*/
    public static  T WrappingSub<T, TSigned>(T value, T sub)
        where T : INumber<T>
        where TSigned : ISignedNumber<TSigned>
    {
        var val = ToSigned<T, TSigned>(value);
        var subVal = ToSigned<T, TSigned>(sub);
        val = - val - subVal;
        return ToUnsigned<T, TSigned>(val);
    }

    public static T WrappingAdd<T>(T value, T add) where T : INumber<T>
    {
        return value + add;
    }

    private static  TSigned ToSigned<T, TSigned>(T value)
        where T : INumber<T>
        where TSigned : ISignedNumber<TSigned>
    {
        return TSigned.CreateTruncating(value);
    }

    private static T ToUnsigned<T, TSigned>(TSigned value)
        where T : INumber<T>
        where TSigned : ISignedNumber<TSigned>
    {
        return T.CreateTruncating(value);
    }
}