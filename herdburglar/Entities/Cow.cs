using System;

using Nez;

namespace herdburglar
{
	public class Cow : Entity
	{
		public Cow()
		{

		}

		public override void onAddedToScene()
		{
			base.onAddedToScene();

			tag = (int)Tags.Cow;
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
