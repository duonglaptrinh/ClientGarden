mergeInto(LibraryManager.library, {
  
  CreatePeerFromOfferJs : function(id,sdpOffer,config){
    CreatePeerFromOfferAsync(UTF8ToString(id),UTF8ToString(sdpOffer),JSON.parse(UTF8ToString(config)));
  },

  CreatePeerJs: function(id,config){
    CreatePeerAsync(UTF8ToString(id),JSON.parse(UTF8ToString(config)));
  },

  DestroyPeerJs : function(id){
    DestroyPeerAsync(UTF8ToString(id));
  },
  
  OnOpenVoiceCallJs : function(){
    console.log("OnOpenVoiceCallJs")
    OnOpenVoiceCall();
  },

  OnCloseVoiceCallJs : function(){
    console.log("OnCloseVoiceCallJs")
    OnCloseVoiceCall();
    DestroyAllPeer();
  },

  FeedAnswerJs: function(id,sdpAnswer){
    FeedAnswerAsync(UTF8ToString(id),UTF8ToString(sdpAnswer));
  },

  FeedIceCandidateJs : function(id,iceCandidate){
    FeedIceCandidateAsync(UTF8ToString(id),UTF8ToString(iceCandidate));
  },

});