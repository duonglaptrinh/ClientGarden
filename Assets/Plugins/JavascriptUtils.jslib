mergeInto(LibraryManager.library, {

  GetWindowWidth: function() {
      return window.innerWidth;
  },

  GetWindowHeight: function() {
      return window.innerHeight;
  },

  DownloadImage: function(imageUrl) {
     DownloadImageByUrl(UTF8ToString(imageUrl));
  },
});