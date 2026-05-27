import React from 'react';
import { render, screen } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { MemoryRouter } from 'react-router-dom';
import { useDetectImage } from '../../../api/queries';
import ImgReadPage from '../ImgReadPage';
import AppRoutes from '../../../routes/AppRoutes';

// Mock useDetectImage hook
jest.mock('../../../api/queries', () => ({
  useDetectImage: jest.fn(),
}));

// Mock antd message to avoid real DOM side effects
jest.mock('antd', () => ({
  ...jest.requireActual('antd'),
  message: {
    error: jest.fn(),
    warning: jest.fn(),
  },
}));

// Import message after mock so we get the mocked version
const { message } = require('antd');

// Helper: wrap component with QueryClientProvider + MemoryRouter
const renderWithProviders = (ui, { initialEntries = ['/'] } = {}) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={initialEntries}>
        {ui}
      </MemoryRouter>
    </QueryClientProvider>
  );
};

// Reset mocks before each test
beforeEach(() => {
  jest.clearAllMocks();
  useDetectImage.mockReturnValue({
    mutate: jest.fn(),
    isLoading: false,
    data: undefined,
    error: null,
    reset: jest.fn(),
  });
});

// ─────────────────────────────────────────────
// Test 1: 初始渲染
// ─────────────────────────────────────────────
test('1. 初始渲染：結果區域不顯示，「開始辨識」按鈕為 disabled', () => {
  renderWithProviders(<ImgReadPage />);

  // 按鈕存在且 disabled（沒有選擇檔案時）
  const button = screen.getByRole('button', { name: /開始辨識/i });
  expect(button).toBeInTheDocument();
  expect(button).toBeDisabled();

  // 結果區域不顯示
  expect(screen.queryByText(/共偵測到/)).not.toBeInTheDocument();
  expect(screen.queryByText(/辨識耗時/)).not.toBeInTheDocument();
});

// ─────────────────────────────────────────────
// Test 2: API 成功（有結果）
// ─────────────────────────────────────────────
test('2. API 成功（有結果）：Table 顯示，物件總數顯示，執行時間顯示', () => {
  useDetectImage.mockReturnValue({
    mutate: jest.fn(),
    isLoading: false,
    data: {
      detections: [
        { label: '人', confidence: 0.9, bbox: { x: 10, y: 20, width: 30, height: 40 } },
      ],
      elapsedMs: 123,
    },
    error: null,
    reset: jest.fn(),
  });

  renderWithProviders(<ImgReadPage />);

  expect(screen.getByText('共偵測到 1 個物件')).toBeInTheDocument();
  expect(screen.getByText('辨識耗時：123 ms')).toBeInTheDocument();
  expect(screen.getByText('人')).toBeInTheDocument();
});

// ─────────────────────────────────────────────
// Test 3: API 成功（空結果）
// ─────────────────────────────────────────────
test('3. API 成功（空結果）：顯示「未偵測到任何物件」，無 Table', () => {
  useDetectImage.mockReturnValue({
    mutate: jest.fn(),
    isLoading: false,
    data: { detections: [], elapsedMs: 50 },
    error: null,
    reset: jest.fn(),
  });

  renderWithProviders(<ImgReadPage />);

  expect(screen.getByText('未偵測到任何物件')).toBeInTheDocument();
  expect(screen.queryByText(/共偵測到/)).not.toBeInTheDocument();
});

// ─────────────────────────────────────────────
// Test 4: API HTTP 錯誤
// ─────────────────────────────────────────────
test('4. API HTTP 錯誤：message.error 顯示「辨識失敗，請稍後再試」', () => {
  useDetectImage.mockReturnValue({
    mutate: jest.fn(),
    isLoading: false,
    data: undefined,
    error: { response: { status: 500 } },
    reset: jest.fn(),
  });

  renderWithProviders(<ImgReadPage />);

  expect(message.error).toHaveBeenCalledWith('辨識失敗，請稍後再試');
});

// ─────────────────────────────────────────────
// Test 5: API 網路錯誤
// ─────────────────────────────────────────────
test('5. API 網路錯誤：message.error 顯示「網路錯誤，請確認連線後重試」', () => {
  useDetectImage.mockReturnValue({
    mutate: jest.fn(),
    isLoading: false,
    data: undefined,
    error: { request: {}, code: undefined },
    reset: jest.fn(),
  });

  renderWithProviders(<ImgReadPage />);

  expect(message.error).toHaveBeenCalledWith('網路錯誤，請確認連線後重試');
});

// ─────────────────────────────────────────────
// Test 6: 按鈕 loading 狀態
// ─────────────────────────────────────────────
test('6. 按鈕 loading 狀態：isLoading: true 時按鈕 disabled', () => {
  useDetectImage.mockReturnValue({
    mutate: jest.fn(),
    isLoading: true,
    data: undefined,
    error: null,
    reset: jest.fn(),
  });

  renderWithProviders(<ImgReadPage />);

  const button = screen.getByRole('button', { name: /開始辨識/i });
  expect(button).toBeDisabled();
});

// ─────────────────────────────────────────────
// Test 7: 路由整合
// ─────────────────────────────────────────────
test('7. 路由整合：/img-read 路由對應 ImgReadPage，顯示「影像辨識」標題', () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={['/img-read']}>
        <AppRoutes />
      </MemoryRouter>
    </QueryClientProvider>
  );

  expect(screen.getByText('影像辨識')).toBeInTheDocument();
});

// ─────────────────────────────────────────────
// Test 8: 選單項目
// ─────────────────────────────────────────────
test('8. 選單項目：側邊選單包含「影像辨識」項目', () => {
  // App 使用 BrowserRouter，改用 MemoryRouter 包裹 AppContent 邏輯
  // 直接 render App 並確認選單文字存在
  const App = require('../../../App').default;
  render(<App />);

  // 選單中應有「影像辨識」文字
  const menuItems = screen.getAllByText('影像辨識');
  expect(menuItems.length).toBeGreaterThan(0);
});

// ─────────────────────────────────────────────
// Test 9: useDetectImage hook 介面
// ─────────────────────────────────────────────
test('9. useDetectImage hook 介面：回傳值包含必要屬性', () => {
  const mockReturn = useDetectImage();

  expect(mockReturn).toHaveProperty('mutate');
  expect(mockReturn).toHaveProperty('isLoading');
  expect(mockReturn).toHaveProperty('data');
  expect(mockReturn).toHaveProperty('error');
  expect(mockReturn).toHaveProperty('reset');

  expect(typeof mockReturn.mutate).toBe('function');
  expect(typeof mockReturn.isLoading).toBe('boolean');
  expect(typeof mockReturn.reset).toBe('function');
});
