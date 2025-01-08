# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.4] - 2024-10-04

### Added

 - [Editor] 自动生成代码：增加对于标记了System.Obsolete的Component类的过滤

### Changed
 
### Removed

### Fixed

## [0.0.3] - 2024-06-25

### Added

### Changed

 - [Runtime] Entity：封装Entity和World，组合成新的Entity结构体
 
### Removed

### Fixed

## [0.0.2] - 2024-06-17

### Added

 - [Both] Component：增加IComponentInitializer用于组件初始化
 - [Runtime] System：增加UnwantedType辅助过滤Entity
 - [Editor] 自动生成代码：增加自定义Gizmos绘制函数支持

### Changed

 - [Editor] 自动生成代码：修改的搜索范围为源目录+生成目录
 - [Editor] 自动生成代码：排除Assembly-CSharp程序集
 - [Editor] 自动生成代码：ConfigScript修改为默认不生成，包含ConfigField字段标记或标ConfigClass类标记时生成
 - [Editor] 自动生成代码：自动生成World类修改为自动生成初始化Initializer类
 
### Removed

 - [Editor] 自动生成代码：删除InitField标记，内容合并到ConfigField标记中

### Fixed

 - [Editor] 自动生成代码：修复Component中默认值无法应用到ConfigScript中的问题

## [0.0.1] - 2024-01-14

## [0.0.0] - 2023-10-06