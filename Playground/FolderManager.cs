using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace dsm
{
    public class FolderManager
    {
        ReaderWriter readerWriter;

        public FolderManager()
        {
            readerWriter = new ReaderWriter();
        }

        public List<string> getImages()
        {
            return readerWriter.getImages();
        }

        public void addImage(string imageName)
        {
            readerWriter.addImage(imageName);
        }

        public void removeImage()
        {
            throw new NotImplementedException();
        }
    }
}
