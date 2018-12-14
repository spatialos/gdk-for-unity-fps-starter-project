using System.Collections.Generic;
using Fps;

public interface ITileProvider
{
	List<TileEnabler> LevelTiles { get; }
}
