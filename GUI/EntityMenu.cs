using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using DefaultEcs;
using Godot;
using Tycoon.Components;
using Tycoon.Systems;
using Tycoon.Systems.Helpers;

namespace Tycoon.GUI;

public partial class EntityMenu : PanelContainer
{
	private readonly INodeEntityMapper _nodeEntityMapper;
	private readonly VBoxContainer _container = new();
	private readonly Map _map;
	private ShapeCast2D _shapeCast;
	private Entity _entity;
	private readonly ISubject<Entity> _entityObservable = new Subject<Entity>();

	public EntityMenu(INodeEntityMapper nodeEntityMapper, Map map)
	{
		_nodeEntityMapper = nodeEntityMapper;
		_map = map;
		_shapeCast = new ShapeCast2D
		{
			CollideWithAreas = true,
			Shape = new CircleShape2D { Radius = 1 },
			Enabled = false,
		};
		Visible = false;
	}

	public override void _Ready()
	{
		SetPhysicsProcess(false);
		SetProcess(false);
		_map.AddChild(_shapeCast);

		var margin = new MarginContainer();
		margin.AddThemeConstantOverride("margin_top", 10);
		margin.AddThemeConstantOverride("margin_left", 10);
		margin.AddThemeConstantOverride("margin_right", 10);
		margin.AddThemeConstantOverride("margin_bottom", 10);
		margin.AddChild(_container);
		AddChild(margin);

		AddButton("Close", () => _entity = default);

		AddLabel(entity => $"Name: {entity.Get<Node2D>().Name}");

		{
			var visibility = _entityObservable.Select(entity => entity.Has<Inventory>());
			AddSeparator(visibility);
			AddLabel(visibility, entity =>
			{
				var inventoryCapacity = entity.Get<InventoryCapacity>().Value;
				var usedInventoryCapacity = inventoryCapacity - entity.Get<RemainingInventorySpace>();
				return $"Inventory ({usedInventoryCapacity}/{inventoryCapacity})";
			});
			AddLabel(_entityObservable.Select(entity => entity.Has<Inventory>() && entity.Get<Inventory>().Value.Count > 0), entity =>
			{
				var stringBuilder = new StringBuilder();

				foreach (var goodAndAmount in entity.Get<Inventory>().Value)
				{
					stringBuilder.AppendLine($"{goodAndAmount.Key}: {goodAndAmount.Value}");
				}

				return stringBuilder.ToString();
			});
		}

		{
			var visibility = _entityObservable.Select(entity => entity.Has<CanNotWorkReason>());
			AddSeparator(visibility);
			AddLabel(visibility, entity => $"Can't work because: {entity.Get<CanNotWorkReason>()}");
		}

		{
			var visibility = _entityObservable.Select(entity => entity.Has<MaximumWorkers>());
			AddSeparator(visibility);
			AddLabel(visibility, entity =>
			{
				var employeeCount = WorkplaceHelper.GetWorkerCount(entity);
				return $"Workers: {employeeCount}/{entity.Get<MaximumWorkers>().Value}";
			});
		}

		{
			var visibility = _entityObservable.Select(entity => entity.Has<Worker>());
			AddSeparator(visibility);

			AddLabel(_entityObservable.Select(entity => entity.Has<Worker>() && !entity.Get<Worker>().Workplace.IsAlive),
			                                 _ => "Unemployed");

			AddButton(_entityObservable.Select(entity => entity.Has<Worker>() && entity.Get<Worker>().Workplace.IsAlive), entity =>
			{
				var workplace = entity.Get<Worker>().Workplace;
				return workplace.Get<Node2D>().Name;
			}, () =>  _entity = _entity.Get<Worker>().Workplace);
		}

		AddButton("Delete", () => _entity.Dispose());
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputActions.MouseclickLeft))
		{
			SetPhysicsProcess(true);
			_shapeCast.GlobalPosition = _shapeCast.GetGlobalMousePosition();
			_shapeCast.Enabled = true;
			GetViewport().SetInputAsHandled();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_shapeCast.IsColliding())
		{
			var collision = _shapeCast.GetCollider(0);
			var entityNode = (Node2D)collision;
			_entity = _nodeEntityMapper.GetEntity(entityNode);
			Visible = true;
			SetProcess(true);
		}

		SetPhysicsProcess(false);
		_shapeCast.Enabled = false;
	}

	public override void _Process(double delta)
	{
		if (!_entity.IsAlive)
		{
			Visible = false;
			return;
		}

		_entityObservable.OnNext(_entity);
	}

	private void AddLabel(Func<Entity, string> textSelector)
	{
		var label = new Label();
		_container.AddChild(label);
		_entityObservable
			.Select(textSelector)
			.DistinctUntilChanged()
			.Subscribe(text => label.Text = text);
	}

	private void AddLabel(IObservable<bool> visibilityObservable, Func<Entity, string> textSelector)
	{
		var label = new Label();
		_container.AddChild(label);
		visibilityObservable.DistinctUntilChanged().Subscribe(visible => label.Visible = visible);
		_entityObservable
			.WithLatestFrom(visibilityObservable)
			.Where(x => x.Second)
			.Select(x => textSelector(x.First))
			.DistinctUntilChanged()
			.Subscribe(text => label.Text = text);
	}

	private void AddSeparator(IObservable<bool> visibilityObservable)
	{
		var separator = new HSeparator();
		_container.AddChild(separator);
		visibilityObservable
			.DistinctUntilChanged()
			.Subscribe(visible => separator.Visible = visible);
	}

	private void AddButton(IObservable<bool> visibilityObservable, Func<Entity, string> textSelector, Action action)
	{
		var button = new Button();
		_container.AddChild(button);
		button.Pressed += action;
		visibilityObservable
			.DistinctUntilChanged()
			.Subscribe(visible => button.Visible = visible);
		_entityObservable
			.WithLatestFrom(visibilityObservable)
			.Where(x => x.Second)
			.Select(x => textSelector(x.First))
			.DistinctUntilChanged()
			.Subscribe(text => button.Text = text);
	}

	private void AddButton(string text, Action action)
	{
		var button = new Button();
		_container.AddChild(button);
		button.Text = text;
		button.Pressed += action;
	}
}
