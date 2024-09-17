using Google.FlatBuffers;
using Town.Tile;

public class TownMapTilesFBBuilder
{
    public byte[] data;
    public ByteBuffer dataBuffer;

    private Offset<Tile> offset;
    private FlatBufferBuilder _builder = new FlatBufferBuilder(1024);   

    public TownMapTilesFBBuilder(TilePoco tile)
    {
        Tile.StartTile(_builder);

        Tile.AddTileId(_builder, tile.tileId);
        Tile.AddBuildable(_builder, tile.buildable);
        Tile.AddBaseBuildingValue(_builder, tile.baseBuildingValue);
        Tile.AddBaseBuildingRootTile(_builder, tile.baseBuildingRootTile);
        Tile.AddTerrain(_builder, (Town.Tile.TileTerrainType)tile.terrain);

        offset = Tile.EndTile(_builder);

        Tile.FinishTileBuffer(_builder, offset);

        data = _builder.SizedByteArray();
        dataBuffer = _builder.DataBuffer;
    }
}
