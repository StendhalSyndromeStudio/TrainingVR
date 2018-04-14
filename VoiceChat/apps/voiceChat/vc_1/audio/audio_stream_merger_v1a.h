#ifndef AUDIO_STREAM_MERGER_V1A_H
#define AUDIO_STREAM_MERGER_V1A_H

#include <mutex>
#include <voice_chat/audio_stream_merger.h>

namespace vc_1 {

  class AudioStreamMergerUtils;

  class AudioStreamMerger_v1a: public voice_chat::AudioStreamMerger
  {

    voice_chat::AudioFormat   _format;

    mutable StreamPtr         _output;
    mutable QList<StreamPtr>  _input;

    mutable std::mutex        _outMutex;
    mutable std::mutex        _inputMutex;

    AudioStreamMergerUtils    *_utils;

    friend class AudioStreamMergerUtils;
  public:
    AudioStreamMerger_v1a();
    ~AudioStreamMerger_v1a();

    // AudioStreamMerger interface
  public:
    virtual voice_chat::AudioFormat format() const;
    virtual StreamPtr output() const;
    virtual QList<StreamPtr> input() const;
    virtual int count() const;
    virtual void setFormat(voice_chat::AudioFormat format);
    virtual void setOutput(const StreamPtr &stream);
    virtual void add(const StreamPtr &stream);
    virtual void remove(const StreamPtr &stream);
  };

}



#endif // AUDIO_STREAM_MERGER_V1A_H
