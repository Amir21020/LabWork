CREATE DATABASE university;

USE university;

CREATE TABLE departments (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name VARCHAR(255) NOT NULL,
    budget DECIMAL(15, 2) DEFAULT 0.00,
    established_at DATETIME NOT NULL,
    is_active BIT DEFAULT 1
);

CREATE TABLE groups (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    group_name VARCHAR(100) NOT NULL,
    average_score DECIMAL(3, 2) DEFAULT 0.00,
    created_at DATETIME NOT NULL,
    is_active BIT DEFAULT 1,
    department_id UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (department_id) REFERENCES departments(id)
);

CREATE TABLE students (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    scholarship DECIMAL(10, 2) DEFAULT 0.00,
    phone_number VARCHAR(25) UNIQUE NOT NULL,
    enrollment_date DATETIME NOT NULL,
    is_active BIT DEFAULT 1,
    group_id UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (group_id) REFERENCES groups(id)
);

CREATE TABLE teachers (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    salary DECIMAL(10, 2) NOT NULL,
    hire_date DATETIME NOT NULL,
    is_active BIT DEFAULT 1,
);

CREATE TABLE lessons (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    subject_name VARCHAR(255) NOT NULL,
    lesson_time DATETIME NOT NULL,
    is_online BIT DEFAULT 0,
    teacher_id UNIQUEIDENTIFIER NOT NULL,
    group_id UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (teacher_id) REFERENCES teachers(id),
    FOREIGN KEY (group_id) REFERENCES groups(id)
);