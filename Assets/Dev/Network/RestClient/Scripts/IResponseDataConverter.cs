
public interface IResponseRawDataConverter<T>
{
    T Convert(byte[] rawData);
}
