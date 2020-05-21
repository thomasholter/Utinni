#pragma once

#include "utinni.h"

namespace utinni
{
class UTINNI_API Terrain
{
public:
    static Terrain* get();

    void setTimeOfDay(float time);
    float getTimeOfDay();
    int getWeatherIndex();
    void setWeatherIndex(int weatherIndex);
    const char* getPath();
};
};