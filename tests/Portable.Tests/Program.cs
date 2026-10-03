using System;
using EyeLens;
using Nokia.Graphics.Imaging;
class Program {
 static void Main() {
  var input=new uint[]{0xff000000,0xffffffff,0xff0000ff,0x7f000000,0x00112233,0xff00ff00};
  var output=new uint[input.Length];
  new CustomEffect(null).ProcessForTest(new PixelRegion{ImagePixels=input,Width=3,Rows=2,Stride=3},new PixelRegion{ImagePixels=output});
  for(int i=0;i<input.Length;i++) {
   if((input[i]&0xff000000)!=(output[i]&0xff000000))throw new Exception("Alpha lost at pixel "+i);
   uint b=output[i]&255,g=(output[i]>>8)&255,r=(output[i]>>16)&255;
   if(b!=g||g!=r)throw new Exception("Output is not grayscale");
  }
  if(output[0]!=0xffffffff || (output[1]&0xffffff)>0x010101 || output[2]!=0xffededed)throw new Exception("Inverse luminance incorrect");
  var padded=new uint[]{123,0xff000000,456,789,0xff000000,321};var target=new uint[6];
  new CustomEffect(null).ProcessForTest(new PixelRegion{ImagePixels=padded,Start=1,Width=1,Rows=2,Stride=3},new PixelRegion{ImagePixels=target});
  if(target[1]!=0xffffffff||target[4]!=0xffffffff||target[0]!=0||target[2]!=0||target[3]!=0||target[5]!=0)throw new Exception("Padding changed");
  Console.WriteLine("PASS: inverse luminance, alpha preservation and row padding");
 }
}
