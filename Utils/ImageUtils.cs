using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Seiun.Utils;

public interface IProcessUtil
{
	public Image ProcessImage(Image image);
}

public static class ImageUtils
{
	public class ProcessPipeline(params IProcessUtil[] processUtils)
	{
		public Image Process(Image image)
		{
			var processedImage = image;
			foreach (var processUtil in processUtils)
			{
				processedImage = processUtil.ProcessImage(processedImage);
			}

			return processedImage;
		}

		public async Task<Image> ProcessAsync(Image image)
		{
			var processedImage = image;
			foreach (var processUtil in processUtils)
			{
				await Task.Run(() => processedImage = processUtil.ProcessImage(processedImage));
			}
			return processedImage;
		}
	}

	public class ResizeUtil(int x, int y) : IProcessUtil
	{
		public Image ProcessImage(Image image)
		{
			image.Mutate(ipc => ipc.Resize(x, y));
			return image;
		}
	}

	public class ProportionallyResizeUtil(int x, int y) : IProcessUtil
	{
		public Image ProcessImage(Image image)
		{
			image.Mutate(ipc => ipc.Resize(new ResizeOptions
			{
				Size = new Size(x, y),
				Mode = ResizeMode.Max
			}));
			return image;
		}
	}
}
