mergeInto(LibraryManager.library, {
  
  SetBorderColorJs : function(id,color){
    SetBorderColor(UTF8ToString(id),UTF8ToString(color));
  },

  SetVoiceCallStatusJs : function(id,voiceCallStatus){
    SetVoiceCallStatus(UTF8ToString(id),UTF8ToString(voiceCallStatus));
  }

});