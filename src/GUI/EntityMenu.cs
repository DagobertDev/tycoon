using System;
using System.Reactive.Disposables;
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
	private readonly Camera _camera;
	private ShapeCast2D _shapeCast;
	public Entity Entity;
	private readonly ISubject<Entity> _entityObservable = new Subject<Entity>();

	public EntityMenu(INodeEntityMapper nodeEntityMapper, Map map, Camera camera)
	{
		_nodeEntityMapper = nodeEntityMapper;
		_map = map;
		_camera = camera;
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

		AddButton("Close", () => Entity = default);
		AddButton("Focus", () => _camera.Focus(Entity), () => _camera.Follow(Entity));
		AddSeparator();
		AddLabel(entity => $"Name: {entity.Get<Node2D>().Name}");

		using (Section(EntityHas<Inventory>()))
		{
			AddLabel(entity =>
			{
				var inventoryCapacity = entity.Get<InventoryCapacity>().Value;
				var usedInventoryCapacity = inventoryCapacity - entity.Get<RemainingInventorySpace>();
				return $"Inventory ({usedInventoryCapacity}/{inventoryCapacity})";
			});
			AddLabel(EntityHas<Inventory>(inventory => inventory.Value.Count > 0), entity =>
			{
				var stringBuilder = new StringBuilder();

				foreach (var goodAndAmount in entity.Get<Inventory>().Value)
				{
					stringBuilder.AppendLine($"{goodAndAmount.Key}: {goodAndAmount.Value}");
				}

				return stringBuilder.ToString();
			});
		}

		using (Section(EntityHas<CanNotWorkReason>()))
		{
			AddLabel(entity => $"Can't work because: {entity.Get<CanNotWorkReason>()}");
		}

		using (Section(EntityHas<MaximumWorkers>(workers => workers > 0)))
		{
			AddLabel(entity =>
			{
				var employeeCount = WorkplaceHelper.GetWorkerCount(entity);
				return $"Workers: {employeeCount}/{entity.Get<MaximumWorkers>().Value}";
			});
		}

		using (Section(EntityHas<Producer>()))
		{
			AddLabel(entity =>
			{
				var producer = entity.Get<Producer>();
				var progress = entity.Get<ProductionProgress>();
				var input = producer.Input != null ? $"{producer.InputAmount} {producer.Input} → " : string.Empty;
				return $"Producing: {input}{producer.OutputAmount} {producer.Output} ({progress.Value:P0})";
			});
		}

		using (Section(EntityHas<Worker>()))
		{
			var employed = EntityHas<Worker>(worker => worker.Workplace.IsAlive);
			var unemployed = EntityHas<Worker>(worker => !worker.Workplace.IsAlive);

			AddLabel(unemployed, _ => "Unemployed");

			AddButton(employed, entity =>
			{
				var workplace = entity.Get<Worker>().Workplace;
				return workplace.Get<Node2D>().Name;
			}, () =>  Entity = Entity.Get<Worker>().Workplace);
		}

		AddSeparator();
		AddButton("Delete", () => Entity.Dispose());
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
			Entity = _nodeEntityMapper.GetEntity(entityNode);
			Visible = true;
			SetProcess(true);
		}

		SetPhysicsProcess(false);
		_shapeCast.Enabled = false;
	}

	public override void _Process(double delta)
	{
		if (!Entity.IsAlive)
		{
			Visible = false;
			return;
		}

		_entityObservable.OnNext(Entity);
	}

	private void AddLabel(IObservable<bool> visibilityObservable, Func<Entity, string> textSelector)
	{
		var label = new Label();
		_container.AddChild(label);
		visibilityObservable
			.DistinctUntilChanged()
			.Subscribe(visible => label.Visible = visible);
		_entityObservable
			.WithLatestFrom(visibilityObservable)
			.Where(x => x.Second)
			.Select(x => textSelector(x.First))
			.DistinctUntilChanged()
			.Subscribe(text => label.Text = text);
	}

	private void AddLabel(Func<Entity, string> textSelector) =>
		AddLabel(_currentSectionVisibility, textSelector);

	private void AddSeparator(IObservable<bool> visibilityObservable)
	{
		var separator = new HSeparator();
		_container.AddChild(separator);
		visibilityObservable
			.DistinctUntilChanged()
			.Subscribe(visible => separator.Visible = visible);
	}

	private void AddSeparator() => AddSeparator(_currentSectionVisibility);

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

	private void AddButton(string text, Action action, Action? doubleClickAction = null)
	{
		var button = new Button();
		_container.AddChild(button);
		button.Text = text;
		button.Pressed += action;

		if (doubleClickAction != null)
		{
			var timeBetweenDoubleClicks = TimeSpan.FromSeconds(0.25f);

			Observable.FromEvent(handler => button.Pressed += handler, handler => button.Pressed -= handler)
				.TimeInterval()
				.Where(unit => unit.Interval <= timeBetweenDoubleClicks)
				.Subscribe(_ => doubleClickAction());
		}

	}

	/// <summary>
	/// Creates a new section that can be used to group items.
	/// Items in a section have the same visibility by default.
	/// </summary>
	private IDisposable Section(IObservable<bool> visibility)
	{
		_currentSectionVisibility = visibility;
		AddSeparator();
		return Disposable.Create(() => _currentSectionVisibility = Observable.Return(true));
	}

	private IObservable<bool> _currentSectionVisibility = Observable.Return(true);

	private IObservable<bool> EntityHas<T>() =>
		_entityObservable.Select(entity => entity.Has<T>());

	private IObservable<bool> EntityHas<T>(Predicate<T> predicate) =>
		_entityObservable.Select(entity => entity.Has<T>() && predicate(entity.Get<T>()));
}
