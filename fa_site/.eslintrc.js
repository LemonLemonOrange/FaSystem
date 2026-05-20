module.exports = {
  // 設定程式執行環境
  // browser: 代表可使用瀏覽器全域變數，如 window、document
  // es2021: 啟用 ES2021 語法與全域變數
  "env": {
    "browser": true,
    "es2021": true
  },

  // 繼承的規則集
  "extends": [
    // ESLint 官方推薦規則
    "eslint:recommended",

    // React 官方推薦規則
    "plugin:react/recommended"
  ],

  // 覆寫特定檔案的設定
  "overrides": [
    {
      // 針對 ESLint 設定檔本身，使用 Node.js 環境
      "env": {
        "node": true
      },

      // 指定要套用此覆寫設定的檔案
      "files": [
        ".eslintrc.{js,cjs}"
      ],

      // 設定此類檔案的 parser 選項
      "parserOptions": {
        // script 表示使用傳統 script 模式，而不是 ES module
        "sourceType": "script"
      }
    }
  ],

  // JavaScript 語法解析設定
  "parserOptions": {
    // 使用目前 ESLint 支援的最新 ECMAScript 版本
    "ecmaVersion": "latest",

    // 將程式碼視為 ES Module，可使用 import/export
    "sourceType": "module"
  },

  // 啟用的外掛
  "plugins": [
    // React 相關 lint 規則
    "react"
  ],

  // React 版本設定
  "settings": {
    "react": {
      "version": "detect"
    }
  },

  // 自訂規則
  "rules": {
    // 要求使用 2 個空格進行縮排，發現不符合時發出警告
    "indent": [
      "warn",
      2,
      { "SwitchCase": 1 }
    ],

    // 要求使用 Windows 的換行風格（CRLF），發現不符合時發出警告
    "linebreak-style": [
      "warn",
      "windows"
    ],

    // 要求字串使用雙引號，發現不符合時發出警告
    "quotes": [
      "warn",
      "double"
    ],

    // 要求語句結尾加上分號，發現不符合時發出警告
    "semi": [
      "warn"
    ],

    // 不建議使用 var 宣告變數，建議改用 let 或 const
    "no-var": [
      "warn"
    ],

    // 要求變數名稱使用駝峰命名法
    // properties: "always" 表示物件屬性名稱也要遵守
    "camelcase": [
      "warn",
      {
        "properties": "always"
      }
    ],

    // 警告已宣告但未使用的變數
    "no-unused-vars": [
      "warn"
    ],

    // 警告空的程式區塊，例如 if、catch、function 裡面沒內容
    "no-empty": [
      "warn"
    ],

    // React 17+ 搭配新的 JSX 轉換時，不需要再手動 import React
    // 所以關閉 JSX 中 React 必須在作用域內的檢查
    "react/react-in-jsx-scope": [
      "off"
    ],

    // 警告不必要的布林值轉換，例如 !!value
    "no-extra-boolean-cast": [
      "warn"
    ],

    // 警告多餘的分號
    "no-extra-semi": [
      "warn"
    ],

    // 警告不安全的 optional chaining 用法
    "no-unsafe-optional-chaining": [
      "warn"
    ]
  }
};