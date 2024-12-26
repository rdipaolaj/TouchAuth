using Mapster;

namespace auth.dto.Mapster;
public static class MapsterConfiguration
{
    public static TypeAdapterConfig Configuration()
    {
        TypeAdapterConfig config = new();


        return config;
    }
}
