using Nez;

using herdburglar.Components.Controllers;

namespace herdburglar
{
	public class Burglar : Entity
	{
		private BurglarController controller;

		public Burglar()
		{

		}

		public override void onAddedToScene()
		{
			base.onAddedToScene();

			controller = addComponent<BurglarController>();

			tag = (int)Tags.Burglar;
		}

		public override void onRemovedFromScene()
		{
			base.onRemovedFromScene();
		}

		public override void update()
		{
			base.update();
		}
	}
}
