using DefaultEcs;
using Godot;

namespace Tycoon.Systems;

public interface INodeEntityMapper
{
	Entity GetEntity(Node2D node);
}
