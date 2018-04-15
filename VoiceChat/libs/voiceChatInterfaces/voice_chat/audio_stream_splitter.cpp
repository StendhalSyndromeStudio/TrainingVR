#include "audio_stream_splitter.h"
using namespace voice_chat;


AudioStreamSplitter::AudioStreamSplitter()
{

}

quint64 AudioStreamSplitter::size() const
{
  return 0;
}

quint64 AudioStreamSplitter::bufferSize() const
{
  return 999999;
}

QByteArray AudioStreamSplitter::read(quint64 size) const
{
  Q_UNUSED(size);
  return QByteArray();
}

quint64 AudioStreamSplitter::read(char *data, quint64 length) const
{
  Q_UNUSED(data);
  return length;
}

QByteArray AudioStreamSplitter::take(quint64 length)
{
  Q_UNUSED(length);
  return QByteArray();
}

quint64 AudioStreamSplitter::take(char *data, quint64 length)
{
  Q_UNUSED(data);
  return length;
}
