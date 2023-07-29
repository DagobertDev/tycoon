using DefaultEcs;
using Godot;

namespace Tycoon.Systems;

public class NodeEntityMapper : INodeEntityMapper
{
	private readonly EntityMap<Node2D> _entityMap;

	public NodeEntityMapper(World world)
	{
		_entityMap = world.GetEntities().AsMap<Node2D>();
	}

	public Entity GetEntity(Node2D node) => _entityMap[node];
}
