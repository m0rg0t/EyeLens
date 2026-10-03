using System;
namespace Nokia.Graphics.Imaging {
 public interface IImageProvider {}
 public class PixelRegion { public uint[] ImagePixels; public int Start,Width,Rows,Stride; public void ForEachRow(Action<int,int,object> row) { for(int y=0;y<Rows;y++) row(Start+y*Stride,Width,null); } }
 public abstract class CustomEffectBase { protected CustomEffectBase(IImageProvider source){} protected abstract void OnProcess(PixelRegion source,PixelRegion target); public void ProcessForTest(PixelRegion source,PixelRegion target){OnProcess(source,target);} }
}
