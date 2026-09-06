'use strict';
// Yalnız statik C# derlemesi: Unity/oyuncu/test yürütücüsü başlatılmaz.
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const root = path.resolve(__dirname, '..', '..');
const output = path.join(root, 'output');
const source = fs.readFileSync(path.join(output, 'verify-unity-compile.cjs'), 'utf8');
const extra = `
const nunit = path.join(root, 'Unity/Library/PackageCache/com.unity.ext.nunit@d8c07649098d/net40/unity-custom/nunit.framework.dll');
const compatibility = files(path.join(data, 'NetStandard/compat/2.1.0/shims/netfx'), '.dll');
compile('Accord and existing archive tests', ['RegionalAccordTests.cs', 'RoleCampaignTests.cs'].map(name => path.join(root, 'Unity/Assets/Tests/Editor', name)), [...standard, ...engine, ...compatibility, runtimeOutput, nunit], path.join(root, 'output/PowerAboveAll.AccordTests.check.dll'));
`;
vm.runInNewContext(source + extra, { require, process, console, __dirname: output });
