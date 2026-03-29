const path = require('path');

module.exports = {
  presets: ['module:@react-native/babel-preset'],
  plugins: [
    '@babel/plugin-transform-export-namespace-from',
    ['dotenv-import', {
      moduleName: '@env',
      path: path.resolve(__dirname, '..', '.env'),
      safe: false,
      allowUndefined: true,
    }],
  ],
};
