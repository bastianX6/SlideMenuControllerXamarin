using System;
using UIKit;
namespace SlideMenuControllerXamarin
{
	public static class SlideMenuControllerExt
	{
		public static SlideMenuController slideMenuController(this UIViewController controller)
		{
			while (controller != null)
			{
				if (controller is SlideMenuController)
				{
					return controller as SlideMenuController;
				}

				controller = controller.ParentViewController;
			}

			return null;
		}

		public static void AddLeftBarButtonWithImage(this UIViewController controller, UIImage image)
		{
			UIBarButtonItem leftBarButton = new UIBarButtonItem(image, UIBarButtonItemStyle.Plain, (object sender, EventArgs e) => {
				controller.ToggleLeft();
			});

			controller.NavigationItem.LeftBarButtonItem = leftBarButton;
		}


		public static void AddRightBarButtonWithImage(this UIViewController controller, UIImage image)
		{
			UIBarButtonItem rightBarButton = new UIBarButtonItem(image, UIBarButtonItemStyle.Plain, (object sender, EventArgs e) =>
			{
				controller.ToggleRight();
			});

			controller.NavigationItem.RightBarButtonItem = rightBarButton;
		}

		public static void ToggleLeft(this UIViewController controller)
		{
			SlideMenuController slideController = controller.slideMenuController();

			if (slideController != null)
			{
				slideController.ToggleLeft();
			}
		}

		public static void ToggleRight(this UIViewController controller)
		{
			SlideMenuController slideController = controller.slideMenuController();

			if (slideController != null)
			{
				slideController.ToggleRight();
			}
		}

		public static void OpenLeft(this UIViewController controller)
		{
			SlideMenuController slideController = controller.slideMenuController();

			if (slideController != null)
			{
				slideController.OpenLeft();
			}
		}

		public static void OpenRight(this UIViewController controller)
		{
			SlideMenuController slideController = controller.slideMenuController();

			if (slideController != null)
			{
				slideController.OpenRight();
			}
		}

		public static void CloseLeft(this UIViewController controller)
		{
			SlideMenuController slideController = controller.slideMenuController();

			if (slideController != null)
			{
				slideController.CloseLeft();
			}
		}

		public static void CloseRight(this UIViewController controller)
		{
			SlideMenuController slideController = controller.slideMenuController();

			if (slideController != null)
			{
				slideController.CloseRight();
			}
		}

		public static void AddPriorityToMenuGesuture(this UIViewController controller, UIScrollView targetScrollView)
		{ 
			SlideMenuController slideController = controller.slideMenuController();
			var recognizers = slideController.View.GestureRecognizers;

			if (slideController != null && recognizers != null)
			{
				foreach (UIGestureRecognizer gesture in recognizers)
				{
					if (gesture is UIPanGestureRecognizer)
					{
						targetScrollView.PanGestureRecognizer.RequireGestureRecognizerToFail(gesture);
					}
				}
			}
		}
	}
}
