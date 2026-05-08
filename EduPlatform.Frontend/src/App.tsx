import { FormEvent, useEffect, useMemo, useState } from 'react'

type View = 'auth' | 'dashboard' | 'users'

type UserResponse = {
  id: string
  name: string
  email: string
  roles: string[]
}

const API_BASE = import.meta.env.VITE_API_BASE ?? 'http://localhost:5211'

function initials(name: string) {
  return name
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase() ?? '')
    .join('')
}

export function App() {
  const [view, setView] = useState<View>('auth')
  const [authTab, setAuthTab] = useState<'login' | 'register'>('login')
  const [me, setMe] = useState<UserResponse | null>(null)
  const [users, setUsers] = useState<UserResponse[]>([])
  const [busy, setBusy] = useState(false)
  const [loadingProfile, setLoadingProfile] = useState(false)
  const [loadingUsers, setLoadingUsers] = useState(false)
  const [query, setQuery] = useState('')
  const [sortMode, setSortMode] = useState<'name' | 'email'>('name')
  const [error, setError] = useState('')

  const isAuthed = me !== null

  async function apiFetch(path: string, init?: RequestInit) {
    return fetch(`${API_BASE}${path}`, {
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
        ...(init?.headers ?? {})
      },
      ...init
    })
  }

  async function loadMe() {
    setLoadingProfile(true)
    const response = await apiFetch('/api/users/me')
    setLoadingProfile(false)

    if (!response.ok) {
      setMe(null)
      return false
    }

    setMe(await response.json())
    return true
  }

  async function loadUsers() {
    setLoadingUsers(true)
    const response = await apiFetch('/api/users')
    setLoadingUsers(false)

    if (!response.ok) {
      throw new Error(await response.text())
    }

    setUsers(await response.json())
  }

  useEffect(() => {
    void loadMe().then((ok) => {
      setView(ok ? 'dashboard' : 'auth')
    })
  }, [])

  useEffect(() => {
    if (view === 'dashboard' && me) {
      void loadUsers().catch((err: unknown) =>
        setError(err instanceof Error ? err.message : 'Ошибка загрузки пользователей')
      )
    }
  }, [view, me])

  const filteredUsers = useMemo(() => {
    const normalized = query.trim().toLowerCase()
    const source = normalized
      ? users.filter((user) =>
          [user.name, user.email, user.roles.join(' ')]
            .join(' ')
            .toLowerCase()
            .includes(normalized)
        )
      : users

    return [...source].sort((left, right) => {
      const leftValue = sortMode === 'name' ? left.name : left.email
      const rightValue = sortMode === 'name' ? right.name : right.email
      return leftValue.localeCompare(rightValue, 'ru')
    })
  }, [users, query, sortMode])

  async function onLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy(true)
    setError('')

    const formData = new FormData(event.currentTarget)
    const response = await apiFetch('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({
        email: String(formData.get('email') ?? ''),
        password: String(formData.get('password') ?? '')
      })
    })

    setBusy(false)

    if (!response.ok) {
      setError(await response.text())
      return
    }

    const ok = await loadMe()
    if (ok) {
      setView('dashboard')
      setAuthTab('login')
    }
  }

  async function onRegister(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy(true)
    setError('')

    const formData = new FormData(event.currentTarget)
    const response = await apiFetch('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({
        name: String(formData.get('name') ?? ''),
        email: String(formData.get('email') ?? ''),
        password: String(formData.get('password') ?? '')
      })
    })

    setBusy(false)

    if (!response.ok) {
      setError(await response.text())
      return
    }

    setAuthTab('login')
  }

  async function onLogout() {
    await apiFetch('/api/auth/logout', { method: 'GET' })
    setMe(null)
    setUsers([])
    setQuery('')
    setView('auth')
    setAuthTab('login')
  }

  async function refreshUsers() {
    try {
      setError('')
      await loadUsers()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка загрузки пользователей')
    }
  }

  function requireAuth(nextView: Exclude<View, 'auth'>) {
    if (!isAuthed) {
      setView('auth')
      setAuthTab('login')
      return
    }
    setView(nextView)
  }

  function renderAuth() {
    return (
      <section className="auth-shell">
        <div className="auth-hero glass">
          <p className="eyebrow">EduPlatform</p>
          <h1>Вход в личный кабинет</h1>
          <p className="lead">
            Сначала авторизация или регистрация, затем доступ к панели, профилю и списку пользователей.
          </p>
          <div className="feature-list">
            <div>cookie auth</div>
            <div>профиль пользователя</div>
            <div>список пользователей</div>
            <div>поиск и сортировка</div>
          </div>
        </div>

        <div className="auth-card glass">
          <div className="auth-tabs">
            <button className={authTab === 'login' ? 'tab active' : 'tab'} onClick={() => setAuthTab('login')} type="button">
              Вход
            </button>
            <button className={authTab === 'register' ? 'tab active' : 'tab'} onClick={() => setAuthTab('register')} type="button">
              Регистрация
            </button>
          </div>

          {authTab === 'login' ? (
            <form onSubmit={onLogin}>
              <label>
                Email
                <input name="email" type="email" placeholder="you@example.com" />
              </label>
              <label>
                Пароль
                <input name="password" type="password" placeholder="••••••••" />
              </label>
              <button className="primary" disabled={busy} type="submit">
                {busy ? 'Входим...' : 'Войти'}
              </button>
            </form>
          ) : (
            <form onSubmit={onRegister}>
              <label>
                Имя
                <input name="name" placeholder="Алексей" />
              </label>
              <label>
                Email
                <input name="email" type="email" placeholder="you@example.com" />
              </label>
              <label>
                Пароль
                <input name="password" type="password" placeholder="••••••••" />
              </label>
              <button className="primary" disabled={busy} type="submit">
                {busy ? 'Создаём...' : 'Создать аккаунт'}
              </button>
            </form>
          )}

          {error && <div className="error">{error}</div>}
        </div>
      </section>
    )
  }

  function renderDashboard() {
    return (
      <section className="workspace">
        <div className="workspace-top glass">
          <div className="profile-head">
            <div className="avatar">{me ? initials(me.name) : 'EP'}</div>
            <div>
              <p className="eyebrow">Панель</p>
              <h1>{me ? `Привет, ${me.name}` : 'Личный кабинет'}</h1>
              <p className="muted">{me ? me.email : loadingProfile ? 'Проверяем сессию...' : 'Нет активной сессии'}</p>
            </div>
          </div>

          <div className="actions">
            <button onClick={() => setView('users')}>Пользователи</button>
            <button onClick={refreshUsers}>Обновить</button>
            <button className="danger" onClick={() => void onLogout()}>
              Выйти
            </button>
          </div>
        </div>

        <div className="stats-grid">
          <article className="stat-card glass">
            <span>Пользователи</span>
            <strong>{users.length}</strong>
          </article>
          <article className="stat-card glass">
            <span>Роли</span>
            <strong>{new Set(users.flatMap((user) => user.roles)).size}</strong>
          </article>
          <article className="stat-card glass">
            <span>Статус</span>
            <strong>{isAuthed ? 'Авторизован' : 'Гость'}</strong>
          </article>
        </div>

        <div className="content-grid">
          <section className="glass section-card">
            <div className="section-head">
              <div>
                <p className="eyebrow">Профиль</p>
                <h2>Мои данные</h2>
              </div>
              <button onClick={() => setView('users')}>Открыть список</button>
            </div>

            {me ? (
              <dl className="definition-list">
                <div>
                  <dt>Имя</dt>
                  <dd>{me.name}</dd>
                </div>
                <div>
                  <dt>Email</dt>
                  <dd>{me.email}</dd>
                </div>
                <div>
                  <dt>Роли</dt>
                  <dd>{me.roles.join(', ') || 'нет'}</dd>
                </div>
              </dl>
            ) : (
              <p className="muted">Профиль загружается...</p>
            )}
          </section>

          <section className="glass section-card">
            <div className="section-head">
              <div>
                <p className="eyebrow">Пользователи</p>
                <h2>Быстрый обзор</h2>
              </div>
              <button onClick={refreshUsers}>Обновить</button>
            </div>

            <div className="toolbar">
              <input
                className="search"
                placeholder="Поиск по имени, email или роли"
                value={query}
                onChange={(event) => setQuery(event.target.value)}
              />
              <select value={sortMode} onChange={(event) => setSortMode(event.target.value as 'name' | 'email')}>
                <option value="name">Сортировка по имени</option>
                <option value="email">Сортировка по email</option>
              </select>
            </div>

            {loadingUsers ? (
              <p className="muted">Загружаем пользователей...</p>
            ) : (
              <div className="list">
                {filteredUsers.slice(0, 5).map((item) => (
                  <article className="list-item" key={item.id}>
                    <div className="list-item-main">
                      <div className="item-title">{item.name}</div>
                      <div className="item-subtitle">{item.email}</div>
                    </div>
                    <div className="chips">
                      {item.roles.map((role) => (
                        <span className="chip" key={role}>
                          {role}
                        </span>
                      ))}
                    </div>
                  </article>
                ))}
                {filteredUsers.length === 0 && <p className="muted">Ничего не найдено.</p>}
              </div>
            )}
          </section>
        </div>
      </section>
    )
  }

  function renderUsers() {
    return (
      <section className="workspace">
        <div className="workspace-top glass">
          <div>
            <p className="eyebrow">Админка</p>
            <h1>Пользователи</h1>
            <p className="muted">Полный список, поиск и сортировка.</p>
          </div>

          <div className="actions">
            <button onClick={() => setView('dashboard')}>Кабинет</button>
            <button onClick={refreshUsers}>Обновить</button>
            <button className="danger" onClick={() => void onLogout()}>
              Выйти
            </button>
          </div>
        </div>

        <div className="glass section-card">
          <div className="toolbar">
            <input
              className="search"
              placeholder="Поиск по имени, email или роли"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
            />
            <select value={sortMode} onChange={(event) => setSortMode(event.target.value as 'name' | 'email')}>
              <option value="name">Сортировка по имени</option>
              <option value="email">Сортировка по email</option>
            </select>
          </div>

          {loadingUsers ? (
            <p className="muted">Загружаем список пользователей...</p>
          ) : (
            <div className="list">
              {filteredUsers.map((item) => (
                <article className="list-item" key={item.id}>
                  <div className="list-item-main">
                    <div className="item-title">{item.name}</div>
                    <div className="item-subtitle">{item.email}</div>
                  </div>
                  <div className="chips">
                    {item.roles.map((role) => (
                      <span className="chip" key={role}>
                        {role}
                      </span>
                    ))}
                    {item.roles.length === 0 && <span className="chip muted-chip">без ролей</span>}
                  </div>
                </article>
              ))}
              {filteredUsers.length === 0 && <p className="muted">Пользователи не найдены.</p>}
            </div>
          )}
        </div>
      </section>
    )
  }

  return (
    <div className="app-shell">
      <header className="topbar glass">
        <div className="brand-block">
          <div className="brand">EduPlatform</div>
          <div className="subtitle">Учебный портал</div>
        </div>

        {isAuthed && (
          <nav className="nav">
            <button onClick={() => setView('dashboard')}>Кабинет</button>
            <button onClick={() => setView('users')}>Пользователи</button>
          </nav>
        )}
      </header>

      <main className="main-content">
        {!isAuthed && renderAuth()}
        {isAuthed && view === 'dashboard' && renderDashboard()}
        {isAuthed && view === 'users' && renderUsers()}
        {error && isAuthed && <div className="error">{error}</div>}
        {!isAuthed && view === 'auth' && loadingProfile && <div className="notice glass">Проверяем вашу сессию...</div>}
      </main>
    </div>
  )
}
