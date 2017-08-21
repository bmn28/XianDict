# XianDict
A Chinese dictionary using WPF.

## Acknowledgements

The two dictionaries currently being used are:

- [CC-CEDICT](https://cc-cedict.org/), a CC-BY-SA-3.0 licensed Chinese-English dictionary
- [重編國語辭典修訂本](http://dict.revised.moe.edu.tw/), a CC-BY-ND-3.0 licensed Chinese-Chinese dictionary created by the Ministry of Education of the Republic of China and [converted into JSON format](https://github.com/g0v/moedict-data) by [g0v](http://g0v.tw) for the [萌典](http://www.moedict.org) project.

Stroke order animation was adapted from [data and code](https://github.com/g0v/zh-stroke-data) created for the 萌典 project.

Simplified character conversion uses data from the [Unihan Database](http://unicode.org/charts/unihan.html) maintained by the Unicode Consortium.

Data for character lookup by radical was obtained from https://github.com/audreyt/moedict-webkit/.

Code for working with pinyin strings was adapted from https://github.com/tsroten/zhon/.

## Features

- Lookup by pinyin and characters, with support for Traditional and Simplified characters, mixing of pinyin and characters, specification of tones, and wildcards
- Character lookup by radical
- Stroke order animation
- Selection definition popup: selecting a term will display a popup with its definition
- Clipboard viewer: clipboard contents containing Chinese characters can be displayed for use with the selection popup
