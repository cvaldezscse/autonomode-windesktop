# autonomode-windesktop
Test automation for windows desktop based apps

# Autonomode.WindowsDesktop

A robust test automation framework for Windows desktop applications including WPF, WinForms, and UWP applications. Built with Clean Architecture principles and designed for scalability and maintainability.

## 🎯 Overview

Autonomode.WindowsDesktop provides a comprehensive testing framework that combines modern .NET practices with powerful automation capabilities through Ranorex integration, BDD support via SpecFlow/Gherkin, and extensive reporting features.

## 🏗️ Architecture

The framework follows Clean Architecture principles with the following layers:

### Core Framework
- **Domain** - Core entities, interfaces, and business rules for test automation
- **Application** - Use cases, orchestration services, and test execution engine

### Infrastructure
- **Infrastructure** - Ranorex integration, UI automation, utilities, helpers, reporting, logging, and event listeners

### Testing Projects
- **Specs** - SpecFlow + Gherkin BDD scenarios
- **Tests** - Unit tests for the framework components

## 🚀 Features

- ✅ **Multi-Platform Desktop Support** - WPF, WinForms, UWP applications
- ✅ **BDD Integration** - SpecFlow and Gherkin support
- ✅ **Ranorex Integration** - Powerful UI automation capabilities
- ✅ **Clean Architecture** - Maintainable and testable codebase
- ✅ **Comprehensive Reporting** - HTML, JSON, XML report generation
- ✅ **Event Listeners** - Test execution hooks and monitoring
- ✅ **Structured Logging** - Detailed test evidence and debugging
- ✅ **NuGet Distribution** - Easy integration via package manager

## 🛠️ Technology Stack

- **.NET 8** - Latest .NET version compatible with Windows 10
- **Ranorex** - UI automation engine
- **SpecFlow** - BDD framework
- **Gherkin** - Business readable test scenarios
- **xUnit** - Unit testing framework

## 📦 Installation

```bash
# Install via NuGet Package Manager
Install-Package Autonomode.WindowsDesktop.Domain
Install-Package Autonomode.WindowsDesktop.Application
Install-Package Autonomode.WindowsDesktop.Infrastructure